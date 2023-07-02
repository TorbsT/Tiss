using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TorbuTils.Giraphe;

namespace Assets.Scripts.Systems
{
    using Extensions;
    internal class WorldGraph : MonoBehaviour
    {
        public static WorldGraph Instance { get; private set; }
        public Graph<Vector2Int> Graph => graph;

        private readonly Graph<Vector2Int> graph = new();
        private readonly HashSet<Vector2Int> loaded = new();

        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            ChunkLoader.Instance.LoadedDelta += JustLoaded;
            JustLoaded(ChunkLoader.Instance.CopyLoaded());
        }
        private void JustLoaded(Dictionary<Vector2Int, GameObject> justLoaded)
        {
            foreach (var loc in justLoaded.Keys)
            {
                loaded.Add(loc);
                foreach (Vector2Int l in loc.Neighbours())
                    if (loaded.Contains(l))
                    {
                        graph.AddEdge(loc, l);
                        graph.AddEdge(l, loc);
                    }
            }
        }
    }
}
