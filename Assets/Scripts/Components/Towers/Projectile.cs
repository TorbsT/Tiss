using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Components.Towers
{
    public class Projectile : Impactor
    {
        public enum CollisionMode
        {
            Any,
            Wall,
            None
        }

        [field: Header("Necessities")]
        [field: SerializeField] public Rigidbody2D CachedRigidbody { get; private set; }
        [field: SerializeField] public Animator CachedAnimator { get; private set; }

        [field: Header("Config")]
        [field: SerializeField]
        public CollisionMode DespawnOnCollision
        { get; set; } = CollisionMode.Any;
        [field: SerializeField] public Vector2 MaxAgeRange { get; set; } = Vector2.one * 10;
        [field: SerializeField] public float DPS { get; set; } = 1f;
        [field: SerializeField, Range(0f, 360f)] public float Spread { get; set; } = 5f;
        [field: SerializeField] public Vector2 InitialSpeedRange { get; set; } = Vector2.one;

        // Other
        public float MaxAge { get; set; }
        public float Age { get; set; }
        public List<Debuff> Debuffs { get; set; } = new();
        public HashSet<Collider2D> CurrentCollisions { get; set; } = new();
        public Action<Projectile, GameObject> ImpactAction { get; set; }
        protected override void Collided(
            ICollection<Collider2D> enterColliders,
            ICollection<Collider2D> exitColliders,
            ICollection<Collider2D> stayColliders
            )
        {
            foreach (var collider in stayColliders)
                CurrentCollisions.Add(collider);
        }
        private void OnValidate()
        {
            if (CachedAnimator == null)
                CachedAnimator = GetComponent<Animator>();
            if (CachedRigidbody == null)
                CachedRigidbody = GetComponent<Rigidbody2D>();
        }
    }
}
