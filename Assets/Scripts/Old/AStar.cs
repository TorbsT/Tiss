using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class AStar : MonoBehaviour//, IPathfindingAlgorithm
    {
        public Vector2Int From;
        public Vector2Int To;
        public GizmosMode mode = GizmosMode.None;
        public List<Step> lastComputation = new();

        void OnDrawGizmos()
        {
            if (mode != GizmosMode.Always) return;
            DrawLastComputation();
        }
        void OnDrawGizmosSelected()
        {
            if (mode != GizmosMode.Selected) return;
            DrawLastComputation();
        }
        private void DrawLastComputation()
        {
            Gizmos.color = Color.yellow;
            for (int i = 1; i < lastComputation.Count; i++)
            {
                Step a = lastComputation[i-1];
                Step b = lastComputation[i];
                Gizmos.DrawLine(CoordinateSystem.LocToPos(a.loc), CoordinateSystem.LocToPos(b.loc));
            }
            Gizmos.DrawSphere(CoordinateSystem.LocToPos(lastComputation[^1].loc), 1f);
        }
        public void Compute()
        {
            Vector2Int from = From;
            Vector2Int to = To;
            List<Step> steps = new();
            steps.Add(new(from, 0));
            while (steps[^1].loc.x != to.x)
            {
                Step step = steps[^1];
                Vector2Int newLoc = step.loc;
                if (step.loc.x < to.x) newLoc.x += 1;
                else newLoc.x -= 1;
                steps.Add(new(newLoc, step.step + 1));
            }
            while (steps[^1].loc.y != to.y)
            {
                Step step = steps[^1];
                Vector2Int newLoc = step.loc;
                if (step.loc.y < to.y) newLoc.y += 1;
                else newLoc.y -= 1;
                steps.Add(new(newLoc, step.step + 1));
            }
            lastComputation = steps;
        }
    }
}