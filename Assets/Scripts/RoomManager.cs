using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    [SerializeField] private int mapSize;
    
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

    public void NextRound()
    {
        Dictionary<Vector2Int, Room> loadedDict = loader.GetLoadedDict();
        foreach (Vector2Int key in loadedDict.Keys)
        {
            Room room = loadedDict[key];
            room.NewRotation += 1;
            room.StartRotationAnimation();
        }
    }
}
