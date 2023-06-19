using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Components.Towers
{
    [RequireComponent(typeof(Acquirer), typeof(Rigidbody2D))]
    public class Bladesaw : MonoBehaviour
    {
        [field: SerializeField] public Acquirer CachedAcquirer { get; private set; }
        [field: SerializeField] public Rigidbody2D Rigidbody { get; private set; }
        [field: SerializeField] public bool SpinWithClock { get; set; } = true;
        public List<Blade> Blades { get; set; } = new();
        public bool Spin { get; set; }
        public float CurrentRPS { get; set; }

        private void OnValidate()
        {
            if (CachedAcquirer == null)
                CachedAcquirer = GetComponent<Acquirer>();
            if (Rigidbody == null)
                Rigidbody = GetComponent<Rigidbody2D>();
        }
    }
}
