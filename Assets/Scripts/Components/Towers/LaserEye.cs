using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Components.Towers
{
    [RequireComponent(typeof(Acquirer), typeof(Look))]
    public class LaserEye : MonoBehaviour
    {
        [field: SerializeField] public LineRenderer LaserVisual { get; private set; }
        [field: SerializeField] public float LaserDPS { get; set; } = 100f;
        public Acquirer CachedAcquirer { get; private set; }
        public Look CachedLook { get; private set; }
        private void Awake()
        {
            CachedAcquirer = GetComponent<Acquirer>();
            CachedLook = GetComponent<Look>();
        }
    }
}
