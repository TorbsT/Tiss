using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Components.Towers
{
    [RequireComponent(typeof(Acquirer))]
    public class IceTower : MonoBehaviour
    {
        [field: SerializeField] public Acquirer CachedAcquirer { get; private set; }
        [field: SerializeField] public Transform ShotOrigin { get; private set; }
        public float Cooldown { get; set; }

        private void OnValidate()
        {
            if (CachedAcquirer == null)
                CachedAcquirer = GetComponent<Acquirer>();
        }
    }
}
