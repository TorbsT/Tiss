using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

namespace Assets.Scripts.Components.Towers
{
    public class Flamethrower : MonoBehaviour
    {
        [field: SerializeField] public Transform ShotOrigin { get; private set; }
        [field: SerializeField] public Look CachedLook { get; private set; }
        [field: SerializeField] public Acquirer CachedAcquirer { get; private set; }
        [field: SerializeField] public VisualEffect CachedVisualEffect { get; private set; }

        public bool Shooting { get; set; }
    }
}
