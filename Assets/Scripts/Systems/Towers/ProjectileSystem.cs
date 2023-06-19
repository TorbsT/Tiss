using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Systems.Towers
{
    using Components.Towers;
    using Components;
    using UnityEngine;
    using Assets.Scripts.Systems.Extensions;

    internal class ProjectileSystem : QuickSystem<Projectile>
    {
        public Projectile Shoot(Transform origin, Projectile prefab)
        {
            GameObject go = PoolSystem.Depool(prefab.gameObject);
            Projectile projectile = go.GetComponent<Projectile>();

            Vector2 initialSpeedRange = prefab.InitialSpeedRange;
            float speed = Random.Range(initialSpeedRange.x, initialSpeedRange.y);
            Vector2 lifetimeRange = prefab.MaxAgeRange;
            float lifetime = Random.Range(lifetimeRange.x, lifetimeRange.y);
            float spread = prefab.Spread/2f;
            float spreadInstance = Random.Range(-spread, spread);
            
            projectile.transform.position = origin.position;
            projectile.transform.rotation = origin.rotation;
            projectile.transform.Rotate(0f, 0f, spreadInstance);
            projectile.CachedRigidbody.velocity = projectile.transform.right*speed;

            projectile.MaxAge = lifetime;
            projectile.Age = 0f;
            projectile.CurrentCollisions = new();
            return projectile;
        }
        protected override void FixedTick(ICollection<Projectile> components)
        {
            foreach (var projectile in components)
            {
                foreach (var hitCollider in projectile.CurrentCollisions.Copy())
                {
                    float damage = projectile.DPS;
                    if (projectile.DespawnOnCollision != Projectile.CollisionMode.Any)
                        damage *= Time.fixedDeltaTime;

                    projectile.ImpactAction?.Invoke(projectile, hitCollider.gameObject);

                    Enemy enemy = hitCollider.GetComponent<Enemy>();
                    if (enemy != null)
                        ((EnemySystem)EnemySystem.Instance).Damage(enemy, damage);

                    if (projectile.DespawnOnCollision == Projectile.CollisionMode.None)
                        continue;
                    if (projectile.DespawnOnCollision == Projectile.CollisionMode.Wall
                        && hitCollider.gameObject.layer != LayerSystem.Instance.WallLayer)
                        continue;

                    // Match, kill this projectile
                    projectile.Age = projectile.MaxAge;
                    break;
                }

                projectile.Age += Time.deltaTime;
                if (projectile.Age >= projectile.MaxAge)
                {
                    projectile.ImpactAction = null;
                    PoolSystem.Enpool(projectile.gameObject);
                }
            }
        }
        protected override void JustDepooledComponent(Projectile projectile)
        {
            projectile.Age = 0f;
            if (projectile.CachedAnimator != null)
                projectile.CachedAnimator.SetTrigger("spawn");
        }

        protected override void Tick(ICollection<Projectile> components)
        {
            
        }
    }
}
