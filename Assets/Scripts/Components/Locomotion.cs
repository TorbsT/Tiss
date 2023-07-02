using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Components
{
    using ScriptableObjects;
    public class Locomotion : MonoBehaviour
    {
        [field: SerializeField] public Rigidbody2D Rb { get; set; } 
        [field: SerializeField] public LocomotionObject Object { get; set; }
        [field: SerializeField] public Vector2 IntendedDirection { get; set; }
        public float AllMultiplier { get; set; } = 1f;
        public float Handicap { get; set; }

        private void OnValidate()
        {
            if (Rb == null)
                Rb = GetComponent<Rigidbody2D>();
        }
    }
}
