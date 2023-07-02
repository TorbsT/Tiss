using Assets.Scripts.Components.Towers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Components
{
    [RequireComponent(typeof(LineRenderer), typeof(BoxCollider2D))]
    public class Laser : Impactor
    {
        private enum GizmosMode
        {
            Always,
            Selected,
            Never
        }
        [field: SerializeField] private GizmosMode Mode { get; set; }
        [field: SerializeField] public Transform LaserOrigin { get; private set; }
        [field: SerializeField] public float LaserVisualShortening { get; private set; } = 0.1f;
        [field: SerializeField] public Transform LaserVisualOrigin { get; private set; }
        [field: SerializeField] public LineRenderer LaserVisual { get; private set; }
        [field: SerializeField] public BoxCollider2D Collider { get; private set; }
        public HashSet<Collider2D> CurrentCollisions { get; set; } = new();
        public float Width { get; set; } = 0.1f;
        public float DPS { get; set; } = 9999f;
        public Vector2 OriginPos { get; set; }
        public Vector2 ImpactPos { get; set; }

        protected override void Collided(
            ICollection<Collider2D> enterColliders,
            ICollection<Collider2D> exitColliders,
            ICollection<Collider2D> stayColliders
            )
        {
            foreach (var collider in stayColliders)
                CurrentCollisions.Add(collider);
        }
        private void Awake()
        {
            Validate();
        }
        private void OnValidate()
        {
            Validate();
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
            if (mode != Mode) return;
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(OriginPos, ImpactPos);
        }
        private void Validate()
        {
            if (LaserOrigin == null)
                LaserOrigin = transform.parent;
            if (LaserVisual == null)
                LaserVisual = GetComponent<LineRenderer>();
            if (Collider == null)
                Collider = GetComponent<BoxCollider2D>();
            if (LaserVisualOrigin == null)
                foreach (var t in GetComponentsInChildren<Transform>())
                    if (t.parent == transform)
                        LaserVisualOrigin = t;
        }
    }
}