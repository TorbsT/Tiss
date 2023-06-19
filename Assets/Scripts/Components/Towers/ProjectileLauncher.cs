using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

namespace Assets.Scripts.Components.Towers
{
    public class ProjectileLauncher : MonoBehaviour
    {
        [field: SerializeField] public VisualEffect CachedVisualEffect { get; private set; }
        [field: SerializeField] public Projectile ProjectilePrefab { get; private set; }
        public float Cooldown { get; set; }
    }
}
