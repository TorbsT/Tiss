using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    using Extensions;
    using Components;
    using TorbuTils.Giraphe;
    internal class WorldNav : MonoBehaviour
    {
        private enum GizmosMode
        {
            None,
            Selected,
            Always
        }
        public static WorldNav Instance { get; private set; }
        public Graph<Vector2Int> Graph => graph;
        private Dictionary<Vector2Int, Room> rooms = new();
        private HashSet<Vector2Int> recentlyUpdated = new();
        private readonly Graph<Vector2Int> graph = new();
        public event Action<ICollection<Vector2Int>> NodesUpdated;
        private void Awake()
        {
            GetComponent<Graph2DVisualizer>().Set(graph);
            Instance = this;
        }
        private void Start()
        {
            ChunkLoader.Instance.LoadedDelta += JustLoaded;
            RotationSystem.Instance.RotationChanged += JustRotated;
            Dictionary<Vector2Int, GameObject> loadedSoFar = ChunkLoader.Instance.CopyLoaded();
            JustLoaded(loadedSoFar);
        }
        private void Update()
        {
            if (recentlyUpdated.Count > 0)
            {
                NodesUpdated?.Invoke(recentlyUpdated);
                recentlyUpdated = new();
            }
        }
        private void JustLoaded(Dictionary<Vector2Int, GameObject> loaded)
        {
            foreach (Vector2Int loc in loaded.Keys)
            {
                rooms.Add(loc, loaded[loc].GetComponent<Room>());
                RefreshNode(loc);
            }
        }
        private void JustRotated(Vector2Int loc, GameObject _)
        {
            RefreshNode(loc);
        }
        private void RefreshNode(Vector2Int loc)
        {
            foreach (Vector2Int n in loc.Neighbours())
            {
                if (rooms.ContainsKey(n) && CanWalkBetween(rooms[loc], rooms[n]))
                {
                    Connect(loc, n);
                } else
                {
                    Disconnect(loc, n);
                }
            }
        }
        public void Connect(Vector2Int a, Vector2Int b)
        {
            if (graph.GetEdgeQuantityBetween(a, b) != 2)
            {
                graph.AddEdge(a, b);
                graph.AddEdge(b, a);
                recentlyUpdated.Add(a);
                recentlyUpdated.Add(b);
            }
        }
        public void Disconnect(Vector2Int a, Vector2Int b)
        {
            if (graph.GetEdgeQuantityBetween(a, b) != 0)
            {
                graph.RemoveEdge(a, b);
                graph.RemoveEdge(b, a);
                recentlyUpdated.Add(a);
                recentlyUpdated.Add(b);
            }
        }
        private bool CanWalkBetween(Room roomGOA, Room roomGOB)
        {
            Room a = roomGOA.GetComponent<Room>();
            Room b = roomGOB.GetComponent<Room>();
            Vector2Int locDiff = b.Loc - a.Loc;
            return DirectionIsAccessible(a, locDiff) && DirectionIsAccessible(b, -locDiff);
        }
        private bool DirectionIsAccessible(Room room, Vector2Int direction)
        {
            int rot = room.Rotation % 4;
            if (rot == 0 && direction == Vector2Int.down) return false;
            if (rot == 1 && direction == Vector2Int.left) return false;
            if (rot == 2 && direction == Vector2Int.up) return false;
            if (rot == 3 && direction == Vector2Int.right) return false;
            return true;
        }
    }
}
