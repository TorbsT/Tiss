using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;

namespace Pathfinding
{
    public class WorldGraphSystem : MonoBehaviour
    {

        // nodes modified by RoomManager, accessed by IPathfindingAlgorithm
        public Dictionary<Vector2Int, HashSet<Vector2Int>> nodes = new();

        public HashSet<Vector2Int> borderNodes = new();
        public GizmosMode borderGizmos;
        public GizmosMode pathGizmos;
        public int nodeCount;
        public int borderCount;
        public ProceduralRoomSystem roomSystem;


        private void OnDrawGizmos()
        {
            DrawGizmos(GizmosMode.Always);
        }
        private void OnDrawGizmosSelected()
        {
            DrawGizmos(GizmosMode.Selected);
        }
        private void DrawGizmos(GizmosMode mode)
        {
            if (mode == pathGizmos)
            {
                Gizmos.color = Color.yellow;
                foreach (Vector2Int node in nodes.Keys)
                {
                    foreach (Vector2Int n in nodes[node])
                    {
                        Gizmos.DrawLine((Vector2)node * ChunkSystem.Instance.chunkSize, (Vector2)n * ChunkSystem.Instance.chunkSize);
                    }
                }
            }
            if (mode == borderGizmos)
            {
                Gizmos.color = Color.red;
                foreach (Vector2Int node in borderNodes)
                {
                    Gizmos.DrawCube((Vector2)node * ChunkSystem.Instance.chunkSize, Vector3.one * ChunkSystem.Instance.chunkSize * 0.8f);
                }
            }
        }
        private void Awake()
        {
            roomSystem = GetComponent<ProceduralRoomSystem>();
        }
        public void Recalculate()
        {
            nodes = new();
            RecalculateDelta(new HashSet<Vector2Int>(), roomSystem.loaded.Keys);
        }
        public void RecalculateDelta(ICollection<Vector2Int> justUnloaded, ICollection<Vector2Int> justLoaded)
        {
            foreach (Vector2Int loc in justUnloaded)
            {
                foreach (Vector2Int n in loc.Neighbours())
                {
                    Disconnect(loc, n);
                    EnsureBorder(n);
                }
                EnsureBorder(loc);
            }
            foreach (Vector2Int loc in justLoaded)
            {
                if (!nodes.ContainsKey(loc))
                    nodes.Add(loc, new());
                foreach (Vector2Int n in loc.Neighbours())
                {
                    if (nodes.ContainsKey(n) && roomSystem.Neighbours(loc, n)) Connect(loc, n);
                    EnsureBorder(n);
                }
                EnsureBorder(loc);
            }
        }

        public void Connect(Vector2Int a, Vector2Int b)
        {
            if (!nodes.ContainsKey(a)) nodes.Add(a, new());
            if (!nodes.ContainsKey(b)) nodes.Add(a, new());
            nodes[a].Add(b);
            nodes[b].Add(a);

            /*
            if (nodes[a].Count == 4) borderNodes.Remove(a);
            else borderNodes.Add(a);
            if (nodes[b].Count == 4) borderNodes.Remove(b);
            else borderNodes.Add(b);
            */

            nodeCount = nodes.Count;
            borderCount = borderNodes.Count;
        }
        public void Disconnect(Vector2Int a, Vector2Int b)
        {
            if (nodes.ContainsKey(a))
            {
                nodes[a].Remove(b);
                if (nodes[a].Count == 0) nodes.Remove(a);
            }
            if (nodes.ContainsKey(b))
            {
                nodes[b].Remove(a);
                if (nodes[b].Count == 0) nodes.Remove(b);
            }

            /*
            if (nodes.ContainsKey(a)) borderNodes.Add(a);
            else borderNodes.Remove(a);
            if (nodes.ContainsKey(b)) borderNodes.Add(b);
            else borderNodes.Remove(b);
            */

            nodeCount = nodes.Count;
            borderCount = borderNodes.Count;
        }
        private void EnsureBorder(Vector2Int loc)
        {
            if (roomSystem.IsBorder(loc))
            {
                borderNodes.Add(loc);
            }
            else borderNodes.Remove(loc);
        }
    }
    public struct Step
    {
        public Vector2Int loc;
        //public Vector2Int prev;
        public int step;

        public Step(Vector2Int loc, int step)
        {
            this.loc = loc;
            this.step = step;
        }
    }
    public interface IPathfindingAlgorithm
    {
        List<Step> ComputeSteps(Vector2Int from, Vector2Int to);
    }

}

