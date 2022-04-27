using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pools;

public class LoaderSystem : MonoBehaviour
{
    [System.Serializable]
    private class Spawn
    {
        [Range(0f, 1f)] public float max;
        public GameObject prefab;
    }
    public static LoaderSystem Instance { get; private set; }
    
    public bool Running => running;
    private ICollection<Vector2Int> locsToLoad;
    private ICollection<Vector2Int> locsToUnload;
    private Dictionary<Vector2Int, Room> dict = new();

    [SerializeField] private List<Spawn> spawns;
    [SerializeField] private bool running;
    [SerializeField] private float fpsPriority = 512f;

    private void Awake()
    {
        Instance = this;
    }
    public Room GetRoomById(Vector2Int id)
    {
        if (!dict.ContainsKey(id)) return null;
        return dict[id];
    }
    public Dictionary<Vector2Int, Room> GetLoadedDictCopy()
    {
        Dictionary<Vector2Int, Room> result = new();
        foreach (Vector2Int key in dict.Keys)
        {
            result.Add(key, dict[key]);
        }
        return result;
    }
    public void StartLoading(ICollection<Vector2Int> locsToLoad, ICollection<Vector2Int> locsToUnload)
    {
        this.locsToLoad = locsToLoad;
        this.locsToUnload = locsToUnload;
        StartCoroutine(StartRoutine());
    }
    private IEnumerator StartRoutine()
    {
        running = true;
        Waiter waiter = new(1f / fpsPriority);

        foreach (Vector2Int loc in locsToLoad)
        {
            Load(loc);
            if (waiter.CheckTime()) yield return null;
        }

        EventSystem.DeclareEvent(Event.LoaderFinished);
        running = false;
    }
    private void Load(Vector2Int loc)
    {
        if (dict.ContainsKey(loc))
        {
            Debug.LogWarning("already loaded");
            return;
        }
        Room room = RoomPool.Instance.Depool();

        room.Id = loc;
        room.transform.position = SquareRoomSystem.Instance.LocToPos(loc);

        dict.Add(loc, room);

        
        float rd = Random.value;
        GameObject prefab = null;
        for (int i = 0; i < spawns.Count; i++)
        {
            if (rd < spawns[i].max)
            {
                prefab = spawns[i].prefab;
                break;
            }
        }

        
        if (prefab != null)
        {
            GameObject go = EzPools.Instance.Depool(prefab);
            Transform t = go.transform;
            RoomDweller dweller = go.GetComponent<RoomDweller>();
            t.position = room.MachineSpawn.position;
            t.localRotation = room.MachineSpawn.localRotation;
        }
    }

}
