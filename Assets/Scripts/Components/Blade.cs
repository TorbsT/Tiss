using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class Blade : Impactor
    {
        [field: SerializeField] public float Damage { get; set; } = 9999;

        public HashSet<Collider2D> CollidedSinceLastCheck { get; set; } = new();
        protected override void Collided(
            ICollection<Collider2D> enterColliders,
            ICollection<Collider2D> exitColliders,
            ICollection<Collider2D> stayColliders)
        {
            foreach (var collider in enterColliders)
            {
                CollidedSinceLastCheck.Add(collider);
            }
        }
    }
}
