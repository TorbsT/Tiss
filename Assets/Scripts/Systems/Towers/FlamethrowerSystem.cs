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
        protected override void JustEnabled()
        {
            SetFloat("heat", 0.01f);
            SetFloat("dmg", 0f);
            SetFloat("burndmg", 10f);
            SetFloat("burnduration", 4f);
        }
        protected override void Tick(ICollection<Flamethrower> turrets)
        {
            foreach (var flamethrower in turrets)
            {
                bool shoot = flamethrower.CachedAcquirer.HasTarget;
                VisualEffect vfx = flamethrower.CachedVisualEffect;
                if (shoot)
                {
                    Projectile projectile =
                    ((ProjectileSystem)ProjectileSystem.Instance)
                        .Shoot(flamethrower.ShotOrigin, projectilePrefab);
                    projectile.Debuffs = new();
                    projectile.Debuffs.Add(new("burn", GetFloat("burnduration").Value,
                        GetFloat("burndmg").Value));
                    projectile.DPS = GetFloat("dmg").Value;
                    Heat(flamethrower, GetFloat("heat").Value * Time.deltaTime);
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
