using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    internal class LayerSystem : MonoBehaviour
    {
        public static LayerSystem Instance { get; private set; }

        [field: SerializeField] public int SpriteUILayer { get; set; }
        [field: SerializeField] public int EnemyLayer { get; set; }
        [field: SerializeField] public int WallLayer { get; set; }

        private void Awake()
        {
            Instance = this;
        }
    }
}
