using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using ExtensionMethods;

public class ProceduralRoomSystem : MonoBehaviour, IEventListener
{
    [Header("CONFIG")]
    [Range(1f, 1000f)] public float fpsPriority = 1f;
    public HashSet<Vector2Int> roomsToLoad = new();
    public HashSet<Vector2Int> roomsToUnload = new();
    public Dictionary<Vector2Int, Room> loaded = new();
    public HashSet<Vector2Int> justLoaded = new();
    public HashSet<Vector2Int> justUnloaded = new();
    public GameObject roomPrefab;
    public GizmosMode loadedGizmos = GizmosMode.Always;

    [Header("DEBUG")]
    public NoiseSubSystem noise;
    public WorldGraphSystem pathfinding;
    public int loadedCount;
    public int justLoadedCount;
    public int justUnloadedCount;
    public int loadQueueCount;
    public int unloadQueueCount;
    public bool running;
    public float lastTime;
    public float allowedDelay;
    

    void OnDrawGizmos()
    {
        DrawGizmos(GizmosMode.Always);
    }
    void OnDrawGizmosSelected()
    {
        DrawGizmos(GizmosMode.Selected);
    }
    void DrawGizmos(GizmosMode mode)
    {
        if (loadedGizmos == mode)
        {
            Gizmos.color = Color.blue;
            foreach (var loc in loaded.Keys)
            {
                Gizmos.DrawCube((Vector2)loc * ChunkSystem.Instance.chunkSize, Vector3.one*ChunkSystem.Instance.chunkSize*0.8f);
            }
        }
    }
    void Update()
    {
        if (noise == null) noise = GetComponent<NoiseSubSystem>();
        if (pathfinding == null) pathfinding = GetComponent<WorldGraphSystem>();
        if (!running && (loadQueueCount > 0 || unloadQueueCount > 0))
        {
            StartCoroutine(Routine());
        }
    }
    IEnumerator Routine()
    {
        running = true;
        justLoaded = new();
        justUnloaded = new();
        HashSet<Vector2Int> load = new();
        foreach (Vector2Int room in roomsToLoad)
        {
            load.Add(room);
        }
        HashSet<Vector2Int> unload = new();
        foreach (Vector2Int room in roomsToUnload)
        {
            unload.Add(room);
        }
        lastTime = Time.realtimeSinceStartup;

        foreach (Vector2Int loc in unload)
        {
            Unload(loaded[loc]);
            loaded.Remove(loc);
            loadedCount = loaded.Count;

            roomsToUnload.Remove(loc);
            unloadQueueCount = roomsToUnload.Count;
            if (TimeToStop()) yield return null;
        }
        foreach (Vector2Int loc in load)
        {
            Load(loc);
            roomsToLoad.Remove(loc);
            loadQueueCount = roomsToLoad.Count;
            if (TimeToStop()) yield return null;
        }

        // Recalculate nodes in pathfinding
        pathfinding.RecalculateDelta(justUnloaded, justLoaded);

        running = false;
    }
    void OnEnable()
    {
        EventSystem.AddEventListener(this, Event.ChunksRecalculated);
        foreach (var loc in ChunkSystem.Instance.chunks)
        {
            Load(loc);
        }
    }
    void OnDisable()
    {
        EventSystem.RemoveEventListener(this, Event.ChunksRecalculated);
        StopAllCoroutines();
        foreach (Room room in FindObjectsOfType<Room>())
        {
            Unload(room);
        }
        loaded = new();
        loadedCount = 0;
        roomsToUnload = new();
        unloadQueueCount = 0;
        roomsToLoad = new();
        loadQueueCount = 0;
        justLoaded = new();
        justLoadedCount = 0;
        justUnloaded = new();
        justUnloadedCount = 0;

        pathfinding.nodes = new();
        pathfinding.borderNodes = new();
        running = false;
    }
    bool TimeToStop()
    {
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow - lastTime > allowedDelay)
        {
            lastTime = timeNow;
            return true;
        }
        return false;
    }
    void IEventListener.EventDeclared(Event e)
    {
        if (e == Event.ChunksRecalculated)
        {
            foreach (var room in ChunkSystem.Instance.justLoaded)
            {
                if (!loaded.ContainsKey(room)) roomsToLoad.Add(room);
                roomsToUnload.Remove(room);
            }
            foreach (var room in ChunkSystem.Instance.justUnloaded)
            {
                roomsToLoad.Remove(room);
                if (loaded.ContainsKey(room)) roomsToUnload.Add(room);
            }
            loadQueueCount = roomsToLoad.Count;
            unloadQueueCount = roomsToUnload.Count;
        }
    }
    private void Load(Vector2Int loc)
    {
        GameObject go = EzPools.Instance.Depool(roomPrefab);
        go.transform.position = (Vector2)loc * ChunkSystem.Instance.chunkSize;
        Room r = go.GetComponent<Room>();
        r.Id = loc;
        r.NewRotation = Mathf.FloorToInt(4*noise.Get(loc));
        r.StartRotationAnimation();
        loaded.Add(loc, r);
        loadedCount = loaded.Count;
        justLoaded.Add(loc);
        justLoadedCount = justLoaded.Count;
    }
    private void Unload(Room room)
    {
        room.GetComponent<Destroyable>().Destroy();
        justUnloaded.Add(room.Id);
        justUnloadedCount = justUnloaded.Count;
    }
    void OnValidate()
    {
        allowedDelay = 1f / fpsPriority;
    }
    public bool IsBorder(Vector2Int loc)
    {
        if (!loaded.ContainsKey(loc)) return false;
        foreach (Vector2Int l in loaded[loc].GetNeighbours())
        {
            if (!loaded.ContainsKey(l)) return true;
        }
        return false;
    }
    public bool Neighbours(Vector2Int a, Vector2Int b)
    {
        if (!loaded.ContainsKey(a)) return false;
        if (!loaded.ContainsKey(b)) return false;

        return loaded[a].IsNeighbour(b) && loaded[b].IsNeighbour(a);
    }
}
