using System;
using System.Collections;
using System.Collections.Generic;
using TorbuTils.Giraphe;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    public class LightGraph : MonoBehaviour
    {
        public static LightGraph Instance { get; private set; }
        public event Action Updated;
        public Graph<Vector2Int> Graph => graph;
        private Graph<Vector2Int> graph;
        private Dictionary<Vector2Int, GameObject> builts = new();

        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            BuildSystem.Instance.JustBuilt += Built;
            builts = new();
            foreach (var keyvalue in BuildSystem.Instance.Towers)
            {
                var loc = keyvalue.Key;
                var go = keyvalue.Value;
                Built(loc, go);
            }
        }
        private void OnEnable()
        {
            graph = new();
            GetComponent<Graph2DVisualizer>().Set(graph);
            WorldNav.Instance.NodesUpdated += NavUpdated;
            
            NavUpdated(WorldNav.Instance.Graph.CopyNodes());

        }
        private void OnDisable()
        {
            WorldNav.Instance.NodesUpdated -= NavUpdated;
            BuildSystem.Instance.JustBuilt -= Built;
        }

        private void NavUpdated(ICollection<Vector2Int> changed)
        {
            Refresh();
        }
        private void Built(Vector2Int loc, GameObject go)
        {
            builts.Add(loc, go);
            Refresh();
        }
        private void Refresh()
        {
            HashSet<(Vector2Int, int)> hotspots = new();
            foreach (var loc in builts.Keys)
                hotspots.Add((loc, 0));
            MultiDijkstra<Vector2Int> multiDijkstra = new(WorldNav.Instance.Graph, hotspots);
            foreach (var _ in multiDijkstra.Solve())
                continue;
            graph = multiDijkstra.ResultGraph;
            GetComponent<Graph2DVisualizer>().Set(graph);
            Updated?.Invoke();
        }
    }
}