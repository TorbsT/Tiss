using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Components.Towers
{
    [RequireComponent(typeof(Acquirer), typeof(Look), typeof(Orientation))]
    public class LaserEye : MonoBehaviour
    {
        public Acquirer CachedAcquirer { get; private set; }
        public Orientation CachedOrientation { get; private set; }
        public Look CachedLook { get; private set; }
        public Laser CachedLaser { get; private set; }
        private void Awake()
        {
            CachedAcquirer = GetComponent<Acquirer>();
            CachedLook = GetComponent<Look>();
            CachedOrientation = GetComponent<Orientation>();
            CachedLaser = GetComponentInChildren<Laser>();
        }
    }
}
