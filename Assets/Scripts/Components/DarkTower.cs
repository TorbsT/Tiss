using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class DarkTower : MonoBehaviour
    {
        [field: Range(0f, 1f)] public float Charge { get; set; }
        public GameObject Orb { get; set; }
    }
}
