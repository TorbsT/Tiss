using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    public class ArrowManager : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;

        private void OnEnable()
        {
            ArrowGraph.Instance.Updated += Refresh;
        }
        private void OnDisable()
        {
            ArrowGraph.Instance.Updated -= Refresh;
        }
        private void Refresh()
        {
            List<Vector3> positions = new();
            Vector2Int current = ArrowGraph.Instance.Origin;
            while (current != Vector2Int.zero)
            {
                positions.Add(new(current.x, current.y, 0f));
                foreach (var neig in ArrowGraph.Instance.Graph.CopyEdgesFrom(current))
                {
                    current = neig;
                    break;
                }
            }
            positions.Add(Vector3.zero);
            lineRenderer.positionCount = positions.Count;
            lineRenderer.SetPositions(positions.ToArray());
        }
    }
}