﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public abstract class Impactor : MonoBehaviour
    {
        public HashSet<Collider2D> CollisionEnters { get; set; } = new();
        public HashSet<Collider2D> CollisionExits { get; set; } = new();
        
        protected abstract void Collided(
            ICollection<Collider2D> enterColliders,
            ICollection<Collider2D> exitColliders);
        private void FixedUpdate()
        {
            if (CollisionEnters.Count > 0 || CollisionExits.Count > 0)
            {
                Collided(CollisionEnters, CollisionExits);
                CollisionEnters.Clear();
                CollisionExits.Clear();
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            CollisionEnters.Add(collision);
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            CollisionExits.Add(collision);
        }
    }
}
