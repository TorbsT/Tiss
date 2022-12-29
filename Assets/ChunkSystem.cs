using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ChunkSystem : MonoBehaviour
{
    public static ChunkSystem Instance { get; private set; }

    [Header("CONFIG")]
    [Range(1f, 1000f)] public float fpsPriority = 1f;
    [Range(1f, 100f)] public float chunkSize = 10f;
    [Range(0, 25)] public int loadDistance = 5;
    [Range(1f, 10f)] public float unloadRatio = 1f;
    public GizmosMode loadCircleGizmos = GizmosMode.Always;
    public GizmosMode chunksGizmos = GizmosMode.Always;
    public GizmosMode justLoadedGizmos = GizmosMode.Always;
    public GizmosMode justUnloadedGizmos = GizmosMode.Always;

    [Header("DEBUG")]
    public Vector2Int center;
    public Vector2Int prevCenter;
    public int prevLoadDistance;
    public int prevUnloadDistance;
    public HashSet<Vector2Int> chunks = new();
    public HashSet<Vector2Int> loadCircle = new();
    public HashSet<Vector2Int> justLoaded = new();
    public HashSet<Vector2Int> justUnloaded = new();
    public bool running = false;
    public float lastTime;
    public float allowedDelay = 0f;
    public int unloadDistance = 5;
    
    private void OnDrawGizmos()
    {
        DoGizmos(GizmosMode.Always);
    }
    private void OnDrawGizmosSelected()
    {
        DoGizmos(GizmosMode.Selected);
    }
    private void DoGizmos(GizmosMode mode)
    {
        if (chunksGizmos == mode)
        {
            Gizmos.color = Color.yellow;
            foreach (var chunk in chunks)
            {
                Gizmos.DrawCube((Vector2)chunk * chunkSize, Vector3.one * chunkSize);
            }
        }
        if (loadCircleGizmos == mode)
        {
            Gizmos.color = Color.green;
            foreach (var chunk in loadCircle)
            {
                Gizmos.DrawCube((Vector2)chunk * chunkSize, Vector3.one * chunkSize*0.9f);
            }
        }
        if (justLoadedGizmos == mode)
        {
            Gizmos.color = Color.cyan;
            foreach (var chunk in justLoaded)
            {
                Gizmos.DrawCube((Vector2)chunk * chunkSize, Vector3.one * chunkSize*0.9f);
            }
        }
        if (justUnloadedGizmos == mode)
        {
            Gizmos.color = Color.red;
            foreach (var chunk in justUnloaded)
            {
                Gizmos.DrawCube((Vector2)chunk * chunkSize, Vector3.one * chunkSize*0.9f);
            }
        }
    }
    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        Instance = this;
        StopAllCoroutines();
        running = false;
        prevCenter = new(1000000, 1000000);
        loadCircle = new();
        prevLoadDistance = int.MaxValue;
        prevUnloadDistance = int.MaxValue;
        chunks = new();
        lastTime = float.MinValue;
    }
    private void OnDisable()
    {
        running = false;
        chunks = new();
        loadCircle = new();
        StopAllCoroutines();
    }
    // Update is called once per frame
    void Update()
    {
        center = new(Mathf.RoundToInt(transform.position.x/chunkSize), Mathf.RoundToInt(transform.position.y/chunkSize));
        if (!running && (center != prevCenter || loadDistance != prevLoadDistance || unloadDistance != prevUnloadDistance))
        {
            prevCenter = center;
            prevLoadDistance = loadDistance;
            prevUnloadDistance = unloadDistance;
            StartCoroutine(Routine());
        }
    }
    IEnumerator Routine()
    {
        running = true;
        lastTime = Time.realtimeSinceStartup;
        loadCircle = new();
        justLoaded = new();
        justUnloaded = new();
        Vector2Int center = prevCenter;
        int loadDistance = prevLoadDistance;
        int unloadDistance = prevUnloadDistance;
        int sqrLoadDistance = loadDistance * loadDistance;
        int sqrUnloadDistance = unloadDistance * unloadDistance;

        // Load new
        for (int x = center.x-loadDistance; x <= center.x + loadDistance; x++)
        {
            for (int y = center.y - loadDistance; y <= center.y + loadDistance; y++)
            {
                if (new Vector2Int(x-center.x, y-center.y).sqrMagnitude <= sqrLoadDistance)
                {
                    loadCircle.Add(new(x, y));
                    if (TimeToStop()) yield return null;
                }
            }
        }
        foreach (Vector2Int chunk in loadCircle)
        {
            if (!chunks.Contains(chunk))
            {
                chunks.Add(chunk);
                justLoaded.Add(chunk);
            }
            
        }

        // Unload old
        foreach (Vector2Int chunk in chunks)
        {
            if (new Vector2Int(chunk.x - center.x, chunk.y - center.y).sqrMagnitude > sqrUnloadDistance)
            {
                justUnloaded.Add(chunk);
            }
            if (TimeToStop()) yield return null;
        }
        foreach (Vector2Int chunk in justUnloaded)
        {
            chunks.Remove(chunk);
        }

        running = false;
        EventSystem.DeclareEvent(Event.ChunksRecalculated);  // RoomSystem needs message every update - uses delta
    }
    private bool TimeToStop()
    {
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow - lastTime > allowedDelay)
        {
            lastTime = timeNow;
            return true;
        }
        return false;
    } 
    private void OnValidate()
    {
        allowedDelay = 1f / fpsPriority;
        unloadDistance = Mathf.RoundToInt(unloadRatio * loadDistance);
    }
}
