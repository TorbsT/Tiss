using UnityEngine;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Systems
{
    internal class ChunkLoader : MonoBehaviour
    {
        public static ChunkLoader Instance { get; private set; }

        [SerializeField] private GameObject roomPrefab;

        public event Action<Dictionary<Vector2Int, GameObject>> LoadedDelta;

        private Dictionary<Vector2Int, GameObject> rooms = new();
        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            ChunkCalculator.Instance.DeltaCalculated += LoadDelta;
            LoadDelta(new HashSet<Vector2Int>(), rooms.Keys);
            LoadDelta(ChunkCalculator.Instance.GetLoaded(), new HashSet<Vector2Int>());
        }
        public Dictionary<Vector2Int, GameObject> CopyLoaded()
        {
            Dictionary<Vector2Int, GameObject> result = new();
            foreach (var key in rooms.Keys)
                result.Add(key, rooms[key]);
            return result;
        }
        private void LoadDelta
            (ICollection<Vector2Int> load, ICollection<Vector2Int> unload)
        {
            Dictionary<Vector2Int, GameObject> loadDict = new();
            foreach (var loc in unload)
            {
                GameObject go = rooms[loc];
                PoolSystem.Enpool(go);
                rooms.Remove(loc);
            }
            foreach (var loc in load)
            {
                GameObject go = PoolSystem.Depool(roomPrefab);
                go.transform.position = (Vector2)loc;
                go.GetComponent<Components.Room>().Loc = loc;
                rooms.Add(loc, go);
                loadDict.Add(loc, go);
            }
            LoadedDelta?.Invoke(loadDict);
        }
    }
}
