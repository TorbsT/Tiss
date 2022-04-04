using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }
    public int GeneratorPower => generatorPower;

    [SerializeField] private int mapSize;
    [SerializeField] private int generatorPower;
    
    private Loader loader;


    private void Awake()
    {
        Instance = this;
        loader = GetComponent<Loader>();
    }

    private void Start()
    {
        HashSet<Vector2Int> locsToLoad = new();
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                locsToLoad.Add(new Vector2Int(x - mapSize / 2, y - mapSize / 2));
            }
        }
        loader.StartLoading(locsToLoad, new HashSet<Vector2Int>());
    }
    public Vector2 LocToPos(Vector2Int loc) => loc * 10;
    public Room PosToRoom(Vector2 pos) => LocToRoom(PosToLoc(pos));
    public Room LocToRoom(Vector2Int loc)
    {
        return loader.GetRoomById(loc);
    }
    public Vector2Int PosToLoc(Vector2 pos) => new(Mathf.RoundToInt(pos.x/10f), Mathf.RoundToInt(pos.y/10f));
    public void NextRound()
    {
        Dictionary<Vector2Int, Room> loadedDict = loader.GetLoadedDict();
        ICollection<Generator> loadedGenerators = loader.GetLoadedGenerators();
        foreach (Vector2Int key in loadedDict.Keys)
        {
            Room room = loadedDict[key];
            room.NewRotation += 1;
            room.StartRotationAnimation();
        }
        foreach (Generator gen in loadedGenerators)
        {
            gen.GetComponent<HP>().Decrease(0f);
            gen.RequestUpdate();
        }

        // Rooms should update their pathfinding
        Pathfinding.PathfindingManager.Instance.NewRound();
    }
}
