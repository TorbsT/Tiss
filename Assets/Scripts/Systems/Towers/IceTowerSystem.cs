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
        [SerializeField] private float heatSpread = -0.1f;
        [SerializeField] private float cooldownBetweenShots = 0.2f;
        protected override void Tick(ICollection<IceTower> turrets)
        {
            foreach (var icetower in turrets)
            {
                icetower.Cooldown -= Time.deltaTime;
                if (icetower.Cooldown > 0f) continue;
                bool shoot = icetower.CachedAcquirer.HasTarget;
                if (shoot)
                {
                    icetower.Cooldown = cooldownBetweenShots;
                    var projectile =
                        ((ProjectileSystem)ProjectileSystem.Instance)
                        .Shoot(icetower.ShotOrigin, projectilePrefab);
                    projectile.ImpactAction += Impact;
                    VFXSystem.Instance.Play
                        (VFXSystem.Instance.IceBullet, projectile.transform);
                    Heat(icetower, heatSpread);
                }
            }
        }
        private void Impact(Projectile projectile, GameObject colliding)
        {
            VFXSystem.Instance.Play
                (VFXSystem.Instance.IceImpact, projectile.transform.position);
            //Rigidbody2D.AddForce(Vector2.down);
        }
    }
}
