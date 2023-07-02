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
        [field: SerializeField] public int Bounty { get; set; } = 10;

        public int InstanceId { get; set; } = -1;
    }
}
