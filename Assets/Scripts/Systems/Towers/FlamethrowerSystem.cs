using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.VFX;

namespace Assets.Scripts.Systems.Towers
{
    using Components.Towers;
    using UnityEngine;

    internal class FlamethrowerSystem : TowerSystem<Flamethrower>
    {
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private float heatSpread = 0.1f;
        protected override void Tick(ICollection<Flamethrower> turrets)
        {
            foreach (var flamethrower in turrets)
            {
                bool shoot = flamethrower.CachedAcquirer.HasTarget;
                VisualEffect vfx = flamethrower.CachedVisualEffect;
                if (shoot)
                {
                    ((ProjectileSystem)ProjectileSystem.Instance)
                        .Shoot(flamethrower.ShotOrigin, projectilePrefab);
                    Heat(flamethrower, heatSpread * Time.deltaTime);
                    if (!flamethrower.Shooting)
                    {
                        flamethrower.Shooting = true;
                        vfx.Play();
                    }
                } else
                {
                    if (flamethrower.Shooting)
                    {
                        flamethrower.Shooting = false;
                        vfx.Stop();
                    }
                }
            }
        }
    }
}
