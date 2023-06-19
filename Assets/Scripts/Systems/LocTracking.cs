using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    using Components;
    internal class LocTracking : MonoBehaviour
    {
        public static LocTracking Instance { get; private set; }

        private readonly Dictionary<Vector2Int, Room> dict = new();
        
        public Room GetRoom(Vector2Int loc)
        {
            if (dict.ContainsKey(loc))
                return dict[loc];
            return null;
        }

        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            ChunkLoader.Instance.LoadedDelta += Loaded;
            Loaded(ChunkLoader.Instance.CopyLoaded());
        }
        private void Loaded(Dictionary<Vector2Int, GameObject> rooms)
        {
            foreach (var loc in rooms.Keys)
            {
                dict.Add(loc, rooms[loc].GetComponent<Room>());
            }
        }
    }
}
