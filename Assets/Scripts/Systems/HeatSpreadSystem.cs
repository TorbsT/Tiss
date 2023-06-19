using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TorbuTils.Giraphe;

namespace Assets.Scripts.Systems
{
    internal class HeatSpreadSystem : MonoBehaviour
    {
        public static HeatSpreadSystem Instance { get; private set; }
        private Dictionary<Vector2Int, float> heatDelta = new();
        [field: SerializeField] public float Falloff { get; set; } = 0.5f;
        [field: SerializeField] public int Radius { get; set; } = 2;

        public void Heat(Vector2Int loc, float amount, float? falloff = null, int? range = null)
        {
            if (range == null)
                range = Radius;
            if (falloff == null)
                falloff = Falloff;
            Dijkstra<Vector2Int> dijkstra = new(WorldGraph.Instance.Graph, loc, range.Value);
            foreach (var _ in dijkstra.Solve()) continue;
            foreach (var node in dijkstra.ResultTree.CopyNodes())
            {
                object costhereSat =
                    dijkstra.ResultTree.GetSatellite(node, Settings.CostSatellite);
                if (costhereSat != null && costhereSat is int costHere)
                {
                    if (!heatDelta.ContainsKey(node))
                        heatDelta.Add(node, 0f);
                    heatDelta[node] += amount * Mathf.Pow(falloff.Value, costHere);
                }
            }
        }
        private void Awake()
        {
            Instance = this;
        }
        private void Update()
        {
            if (heatDelta.Count == 0) return;
            foreach (Vector2Int loc in heatDelta.Keys)
            {
                DisturbanceSystem.Instance.Heat(loc, heatDelta[loc]);
            }
            heatDelta = new();
        }
    }
}
