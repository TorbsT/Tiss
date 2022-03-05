using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pools;

public class Loader : MonoBehaviour
{
    public bool Running => running;
    private ICollection<Vector2Int> locsToLoad;
    private ICollection<Vector2Int> locsToUnload;
    private Dictionary<Vector2Int, Room> dict = new();
    private HashSet<Generator> generators = new();

    [SerializeField] private bool running;
    [SerializeField] private int generatorOdds = 10;
    [SerializeField] private float fpsPriority = 512f;

    public Room GetRoomById(Vector2Int id)
    {
        if (!dict.ContainsKey(id)) return null;
        return dict[id];
    }
    public ICollection<Generator> GetLoadedGenerators()
    {
        HashSet<Generator> result = new();
        foreach (Generator gen in generators)
        {
            result.Add(gen);
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
        foreach (Vector2Int loc in locsToUnload)
        {
            Unload(loc);
            if (waiter.CheckTime()) yield return null;
        }
        foreach (Vector2Int loc in locsToLoad)
        {
            Load(loc);
            if (waiter.CheckTime()) yield return null;
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

        if (Random.Range(0, generatorOdds) == 0)
        {
            Generator gen = GeneratorPool.Instance.Depool();
            gen.MaxPower = RoomManager.Instance.GeneratorPower;
            gen.Place(room);
            generators.Add(gen);
        }

        dict.Add(loc, room);
    }
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

        RoomPool.Instance.Enpool(room);
        dict.Remove(loc);
    }
}
