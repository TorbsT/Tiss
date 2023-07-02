using Assets.Scripts.Components.Towers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Systems.Towers
{
    internal class IceTowerSystem : TowerSystem<IceTower>
    {
        [SerializeField] private Projectile projectilePrefab;

        protected override void JustEnabled()
        {
            SetFloat("heat", -0.01f);
            SetFloat("cooldown", 1.5f);
        }
        protected override void Tick(ICollection<IceTower> turrets)
        {
            foreach (var icetower in turrets)
            {
                icetower.Cooldown -= Time.deltaTime;
                if (icetower.Cooldown > 0f) continue;
                bool shoot = icetower.CachedAcquirer.HasTarget;
                if (shoot)
                {
                    icetower.Cooldown = GetFloat("cooldown").Value;
                    var projectile =
                        ((ProjectileSystem)ProjectileSystem.Instance)
                        .Shoot(icetower.ShotOrigin, projectilePrefab);
                    projectile.ImpactAction += Impact;
                    VFXSystem.Instance.Play
                        (VFXSystem.Instance.IceBullet, projectile.transform);
                    Heat(icetower, GetFloat("heat").Value);
                }
            }
        }
        private void Impact(Projectile projectile, GameObject colliding)
        {
            VFXSystem.Instance.Play
                (VFXSystem.Instance.IceImpact, projectile.transform.position);
        }
    }
}
