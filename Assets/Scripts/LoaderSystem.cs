using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pools;

public class LoaderSystem : MonoBehaviour
{
    public static LoaderSystem Instance { get; private set; }
    
    public bool Running => running;
    private ICollection<Vector2Int> locsToLoad;
    private ICollection<Vector2Int> locsToUnload;
    private Dictionary<Vector2Int, Room> dict = new();

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
    }

}
