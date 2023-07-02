using Assets.Scripts.Components;
using Assets.Scripts.Components.Towers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Systems.Towers
{
    internal class CannonSystem : TowerSystem<Cannon>
    {
        [field: SerializeField] public Projectile ProjectilePrefab { get; private set; }
        protected override void JustEnabled()
        {
            SetFloat("dmg", 200);
            SetFloat("explosionradius", 0.3f);
            SetFloat("explosionforce", 500f);
            SetFloat("cooldown", 3f);
            SetFloat("fov", 110f);
            SetFloat("heat", 0.01f);
        }
        protected override void Tick(ICollection<Cannon> cannons)
        {
            foreach (var cannon in cannons)
            {
                cannon.CachedOrientation.FOV = GetFloat("fov").Value;
                bool shoot = cannon.CachedAcquirer.HasTarget
                    && cannon.Cooldown <= 0f;
                if (shoot)
                {
                    Projectile projectile =
                    ((ProjectileSystem)ProjectileSystem.Instance)
                        .Shoot(cannon.ShotOrigin, ProjectilePrefab);
                    projectile.ImpactAction = Explode;
                    Heat(cannon, GetFloat("heat").Value);
                    cannon.Cooldown = GetFloat("cooldown").Value;
                }
                cannon.Cooldown -= Time.deltaTime;
            }
        }
        private void Explode(Projectile projectile, GameObject victim)
        {
            float explosionRadius = GetFloat("explosionradius").Value;
            ICollection<Collider2D> inRange = Physics2D.OverlapCircleAll(
                projectile.transform.position,
                explosionRadius,
                1 << LayerSystem.Instance.EnemyLayer
                );
            foreach (var collider in inRange)
            {
                Vector2 force = collider.transform.position-projectile.transform.position;
                float distance = force.magnitude;
                float distanceModifier = 1f - (distance / explosionRadius);
                force.Normalize();
                force *= distanceModifier*GetFloat("explosionforce").Value;
                float damage = distanceModifier * GetFloat("dmg").Value;
                ((EnemySystem)EnemySystem.Instance).Damage
                    (collider, damage);
                collider.GetComponent<Rigidbody2D>().AddForce(force);
                collider.GetComponent<Locomotion>().Handicap = 4f;
            }
        }
    }
}
