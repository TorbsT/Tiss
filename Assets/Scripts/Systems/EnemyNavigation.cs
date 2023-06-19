using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TorbuTils.Giraphe;

namespace Assets.Scripts.Systems
{
    internal class EnemyNavigation : MonoBehaviour
    {
        public static EnemyNavigation Instance { get; private set; }
        public Graph<Vector2Int> Graph => navGraph;
        [SerializeField] private int iterationsEachTick = 1000;
        private bool running;
        private bool requestedNew;
        private Graph<Vector2Int> navGraph = new();
        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            WorldNav.Instance.NodesUpdated += GraphUpdated;
            requestedNew = true;
        }
        private void Update()
        {
            if (!running && requestedNew)
            {
                StartCoroutine(nameof(Recalculate));
            }
        }
        private void GraphUpdated(ICollection<Vector2Int> nodes)
        {
            requestedNew = true;
        }
        private IEnumerator Recalculate()
        {
            running = true;
            requestedNew = false;
            Graph<Vector2Int> inputGraph = WorldNav.Instance.Graph;
            Dijkstra<Vector2Int> dijkstra = new(inputGraph, Vector2Int.zero, 1000);
            int i = 1;
            foreach (var _ in dijkstra.Solve())
            {
                i++;
                if (i % iterationsEachTick == 0)
                    yield return null;
            }
                

            navGraph = dijkstra.ResultTree;
            running = false;
            GetComponent<Graph2DVisualizer>().Set(navGraph);
        }
    }
}