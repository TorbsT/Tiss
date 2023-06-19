using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Components.Towers
{
    public class Acquirer : MonoBehaviour
    {
        public enum Mode
        {
            Closest,
            Furthest,
            LeastRotation
        }
        public enum GizmosMode
        {
            None,
            Selected,
            Always
        }
        [field: SerializeField] public Mode TargetMode { get; set; } = Mode.Closest;
        [field: SerializeField] public float Range { get; set; } = 2f;
        [field: SerializeField] public bool Tunnel { get; set; } = true;
        public bool HasTarget => Target != null;
        public Transform Target { get; set; }
        public int TargetId { get; set; } = -1;
        public List<Transform> TargetsInRange { get; set; } = new();
        public List<Transform> TargetsWithLOS { get; set; } = new();
        [field: SerializeField] public Look CachedLook { get; set; }
        [field: SerializeField] public Orientation CachedOrientation { get; set; }

        [SerializeField] private GizmosMode gizmos = GizmosMode.Always;

        private void OnValidate()
        {
            if (CachedLook == null)
                CachedLook = GetComponent<Look>();
            if (CachedLook == null)
                CachedLook = GetComponentInChildren<Look>();
            if (CachedOrientation == null)
                CachedOrientation = GetComponent<Orientation>();
        }
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
            if (!Application.isPlaying) return;
            if (mode != gizmos) return;

            Gizmos.color = Color.green;
            if (Target != null)
            Gizmos.DrawLine(transform.position, Target.position);

            Gizmos.color = Color.red;
            foreach (var target in TargetsInRange)
            {
                if (target == Target) continue;
                if (TargetsWithLOS.Contains(target)) continue;
                Gizmos.DrawLine(transform.position, target.position);
            }

            Gizmos.color = Color.blue;
            foreach (var target in TargetsWithLOS)
            {
                if (target == Target) continue;
                Gizmos.DrawLine(transform.position, target.position);
            }
        }
    }
}