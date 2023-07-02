using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Components.Towers
{
    [RequireComponent(typeof(Look), typeof(Acquirer), typeof(Orientation))]
    public class Cannon : MonoBehaviour
    {
        public Look CachedLook { get; private set; }
        public Acquirer CachedAcquirer { get; private set; }
        public Orientation CachedOrientation { get; private set; }
        [field: SerializeField] public Transform ShotOrigin { get; private set; }
        
        public float Cooldown { get; set; }

        private void Awake()
        {
            Validate();
        }
        private void OnValidate()
        {
            Validate();
        }
        private void Validate()
        {
            if (CachedLook == null)
                CachedLook = GetComponent<Look>();
            if (CachedAcquirer == null)
                CachedAcquirer = GetComponent<Acquirer>();
            if (CachedOrientation == null)
                CachedOrientation = GetComponent<Orientation>();
        }
    }
}
