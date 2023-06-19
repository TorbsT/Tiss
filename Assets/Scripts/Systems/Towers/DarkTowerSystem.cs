using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Systems.Towers
{
    using Components;
    internal class DarkTowerSystem : TowerSystem<DarkTower>
    {
        [field: SerializeField, Range(0f, 1f)]
        public float ChargeSpeed
        { get; set; } = 1f;
        [field: SerializeField, Range(-1f, 4f)]
        public float ShotHeatAmount
        { get; set; } = -1f;
        [field: SerializeField] public GameObject OrbPrefab { get; set; }

        protected override void Tick(ICollection<DarkTower> components)
        {
            foreach (var tower in components)
            {
                tower.Charge += ChargeSpeed * Time.deltaTime;
                if (tower.Charge >= 1f)
                {
                    tower.Charge -= 1f;
                    Heat(tower, ShotHeatAmount);
                }
            }
        }
    }
}
