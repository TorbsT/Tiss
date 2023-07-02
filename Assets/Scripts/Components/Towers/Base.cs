using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Components.Towers
{
    public class Base : Impactor
    {
        HashSet<Collider2D> EnterColliders { get; set; } = new();
        [field: SerializeField] public float Hp { get; set; } = 0f;
        [field: SerializeField] public float MaxHp { get; set; } = 0f;

        protected override void Collided(
            ICollection<Collider2D> enterColliders,
            ICollection<Collider2D> exitColliders,
            ICollection<Collider2D> stayColliders
            )
        {
            foreach (var collider in enterColliders)
            {
                EnterColliders.Add(collider);
            }
        }
    }
}