using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class Enemy : MonoBehaviour
    {
        [field: SerializeField] public float Health { get; set; }
        [field: SerializeField] public float MaxHealth { get; set; } = 100f;

        public int InstanceId { get; set; } = -1;
    }
}
