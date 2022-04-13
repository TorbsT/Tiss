using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pools;

public class Loader : MonoBehaviour
{
    [System.Serializable]
    private class Spawn
    {
        public GameObject prefab;
        [Range(0f, 1f)] public float maxValue;
    }

    public bool Running => running;
    private ICollection<Vector2Int> locsToLoad;
    private ICollection<Vector2Int> locsToUnload;
    private Dictionary<Vector2Int, Room> dict = new();
    private HashSet<RoomDweller> dwellers = new();

    [SerializeField] private bool running;
    [SerializeField] private List<Spawn> spawnChances;
    [SerializeField] private float fpsPriority = 512f;

    public Room GetRoomById(Vector2Int id)
    {
        if (!dict.ContainsKey(id)) return null;
        return dict[id];
    }
    public ICollection<RoomDweller> GetLoadedDwellers()
    {
        HashSet<RoomDweller> result = new();
        foreach (RoomDweller dweller in dwellers)
        {
            result.Add(dweller);
        }
        return result;
    }
    public Dictionary<Vector2Int, Room> GetLoadedDict()
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
        /*
        foreach (Vector2Int loc in locsToUnload)
        {
            Unload(loc);
            if (waiter.CheckTime()) yield return null;
        }
        */
        foreach (Vector2Int loc in locsToLoad)
        {
            Load(loc);
            if (waiter.CheckTime()) yield return null;
        }

        foreach (Room room in dict.Values)
        {
            room.AllOtherRoomsLoaded();
        }
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
        room.transform.position = RoomManager.Instance.LocToPos(loc);
        room.NewRotation = Random.Range(0, 4);
        room.StartRotationAnimation();

        float rd = Random.value;
        GameObject prefab = null;
        for (int i = 0; i < spawnChances.Count; i++)
        {
            if (rd < spawnChances[i].maxValue)
            {
                prefab = spawnChances[i].prefab;
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
            // dunno if tracking dwellers or gameobjects is better
            dwellers.Add(dweller);
        }

        dict.Add(loc, room);
    }
    /*
    private void Unload(Vector2Int loc)
    {
        if (!dict.ContainsKey(loc))
        {
            Debug.LogWarning("already unloaded");
            return;
        }
        Room room = dict[loc];

        foreach (Generator gen in room.transform.GetComponentsInChildren<Generator>())
        {
            generators.Remove(gen);
        }
        // unload generators? idk this whole shit is pretty weird
        RoomPool.Instance.Enpool(room);
        dict.Remove(loc);
    }
    */
}
