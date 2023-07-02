using System;
using System.Collections;
using System.Collections.Generic;
using TorbuTils.Giraphe;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    public class ArrowGraph : MonoBehaviour
    {
        public enum OriginDirection
        {
            NORTH,
            EAST,
            SOUTH,
            WEST
        }
        public static ArrowGraph Instance { get; private set; }
        public event Action Updated;
        public Graph<Vector2Int> Graph { get; private set; }
        public Vector2Int Origin { get; private set; }
        public Vector2Int Spawnpoint { get; private set; }

        [SerializeField] private int minDarknessLevel = 2;
        [SerializeField] private OriginDirection originDirection;
        public void SetOriginDirection(OriginDirection direction)
        {
            originDirection = direction;
            Refresh();
        }

        private void Awake()
        {
            Instance = this;
        }
        private void OnEnable()
        {
            EnemyNavigation.Instance.Updated += EnemyNavUpdated;
            LightGraph.Instance.Updated += LightGraphUpdated;
        }
        private void OnDisable()
        {
            EnemyNavigation.Instance.Updated -= EnemyNavUpdated;
            LightGraph.Instance.Updated -= LightGraphUpdated;
        }
        private void OnValidate()
        {
            if (!Application.isPlaying) return;
            if (EnemyNavigation.Instance == null) return;
            if (LightGraph.Instance == null) return;
            Refresh();
        }
        private void EnemyNavUpdated()
            => Refresh();
        private void LightGraphUpdated()
            => Refresh();
        private void Refresh()
        {
            IComparer<Vector2Int> locComparer = originDirection switch
            {
                OriginDirection.NORTH => new NorthComparer(),
                OriginDirection.EAST => new EastComparer(),
                OriginDirection.SOUTH => new SouthComparer(),
                _ => new WestComparer(),
            };
            List<Vector2Int> sorted = new();
            foreach (var node in EnemyNavigation.Instance.Graph.CopyNodes())
                sorted.Add(node);

            // Remove all that are too close to the base
            foreach (var node in LightGraph.Instance.Graph.CopyNodes())
            {
                object obj = LightGraph.Instance.Graph.GetSatellite(node, Settings.CostSatellite);
                int cost = (int)obj;
                if (cost < minDarknessLevel)
                    sorted.Remove(node);
            }

            // Sort based on location
            sorted.Sort(locComparer);

            // Optimal origin is now the first element
            if (sorted.Count == 0)
            {
                Debug.LogWarning("No rooms were available for spawning");
                return;
            }
            Origin = sorted[0];

            // Get the path from origin to base
            Graph<Vector2Int> arrowGraph = new();
            Vector2Int current = Origin;
            while (current != Vector2Int.zero)
            {
                foreach (var neig in EnemyNavigation.Instance.Graph.CopyEdgesTo(current))
                {
                    arrowGraph.AddEdge(current, neig);
                    current = neig;
                    break;
                }
            }

            // Find best spawn point
            // Criteria: darkness > x, as close as possible
            current = Vector2Int.zero;
            while (current != Origin)
            {
                object obj = LightGraph.Instance.Graph.GetSatellite(current, Settings.CostSatellite);
                if (obj != null)
                {  // Might happen at the start
                    int darkness = (int)obj;
                    if (darkness >= minDarknessLevel)
                        break;
                }

                foreach (var neig in arrowGraph.CopyEdgesTo(current))
                {
                    current = neig;
                }
            }
            Spawnpoint = current;

            GetComponent<Graph2DVisualizer>().Set(arrowGraph);
            Graph = arrowGraph;
            Updated?.Invoke();
        }
        private class SouthComparer : IComparer<Vector2Int> { public int Compare(Vector2Int a, Vector2Int b) => a.y - b.y;}
        private class WestComparer : IComparer<Vector2Int> { public int Compare(Vector2Int a, Vector2Int b) => a.x - b.x;}
        private class EastComparer : IComparer<Vector2Int> { public int Compare(Vector2Int a, Vector2Int b) => b.x - a.x;}
        private class NorthComparer : IComparer<Vector2Int> { public int Compare(Vector2Int a, Vector2Int b) => b.y - a.y;}
    }
}