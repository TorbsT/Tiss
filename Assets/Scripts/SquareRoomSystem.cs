using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareRoomSystem : MonoBehaviour, IEventListener
{
    public static SquareRoomSystem Instance { get; private set; }
    public int GeneratorPower => generatorPower;

    [SerializeField] private int mapSize;
    [SerializeField] private int generatorPower;
    private Dictionary<Vector2Int, Room> dict = new();

    private void Awake()
    {
        Instance = this;
        EventSystem.AddEventListener(this, Event.MasterStarted);
    }
    public Vector2 LocToPos(Vector2Int loc) => loc * 10;
    public Room PosToRoom(Vector2 pos) => LocToRoom(PosToLoc(pos));
    public Room LocToRoom(Vector2Int loc)
    {
        return LoaderSystem.Instance.GetRoomById(loc);
    }
    public Vector2Int PosToLoc(Vector2 pos) => new(Mathf.RoundToInt(pos.x/10f), Mathf.RoundToInt(pos.y/10f));

    private void Reload()
    {
        HashSet<Vector2Int> locsToLoad = new();
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                locsToLoad.Add(new Vector2Int(x - mapSize / 2, y - mapSize / 2));
            }
        }
        GetComponent<LoaderSystem>().StartLoading(locsToLoad, new HashSet<Vector2Int>());
    }
    public void EventDeclared(Event e)
    {
        if (e == Event.MasterStarted)
        {
            Reload();
        }
    }
}
