using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Systems.Towers
{
    using Components.Towers;
    using Components;
    internal class AcquiringSystem : QuickSystem<Acquirer>
    {
        protected override void Tick(ICollection<Acquirer> components)
        {
            HashSet<Enemy> enemies = new();
            foreach (var enemy in EnemySystem.Instance.CopyComponents())
                enemies.Add(enemy);

            foreach (var targeter in components)
            {
                Vector2 origin = targeter.transform.position;
                Collider2D[] colliders = Physics2D.OverlapCircleAll
                    (origin, targeter.Range,
                    1 << LayerSystem.Instance.EnemyLayer);

                List<Transform> inRange = new();
                foreach (var collider in colliders)
                    inRange.Add(collider.transform);

                // Find all in line of sight
                List<Transform> inLOS = new();
                foreach (var t in inRange)
                {
                    Vector2 direction = (Vector2)t.position - origin;
                    if (targeter.CachedOrientation != null)
                    {
                        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        float relativeAngle = Mathf.DeltaAngle(targeter.CachedOrientation.Global, targetAngle);
                        if (Mathf.Abs(relativeAngle) > targeter.CachedOrientation.FOV / 2f)
                            continue;
                    }

                    RaycastHit2D hitData = Physics2D.Raycast
                        (origin, direction, direction.magnitude,
                        1 << LayerSystem.Instance.WallLayer);
                    bool hit =
                        hitData.transform == null;
                    if (!hit) continue;
                    inLOS.Add(t);
                }

                // Find closest and furthest targets
                // Also find least rotation
                Transform closestTarget = null;
                Transform furthestTarget = null;
                Transform leastRotTarget = null;
                float closest = float.MaxValue;
                float furthest = float.MinValue;
                float leastRot = float.MaxValue;
                foreach (var t in inLOS)
                {
                    float dist = ((Vector2)t.position - origin).sqrMagnitude;
                    if (dist < closest)
                    {
                        closestTarget = t;
                        closest = dist;
                    }
                    if (dist > furthest)
                    {
                        furthestTarget = t;
                        furthest = dist;
                    }
                    float rot = Vector2.Angle(targeter.transform.right, (Vector2)t.position - origin);
                    if (rot < leastRot)
                    {
                        leastRotTarget = t;
                        leastRot = rot;
                    }
                }

                // Decide target based on config
                Transform target = null;
                if (targeter.TargetMode == Acquirer.Mode.Closest)
                    target = closestTarget;
                else if (targeter.TargetMode == Acquirer.Mode.Furthest)
                    target = furthestTarget;
                else if (targeter.TargetMode == Acquirer.Mode.LeastRotation)
                    target = leastRotTarget;

                targeter.TargetsInRange = inRange;
                targeter.TargetsWithLOS = inLOS;
                // Change target if not tunneling,
                // or if victim is gone
                if (!targeter.Tunnel ||
                    !inLOS.Contains(targeter.Target))
                {
                    targeter.Target = target;
                    targeter.TargetId = target != null ?
                        target.GetComponent<Enemy>().InstanceId : -1;
                }
                    
                // Look at current target
                if (targeter.CachedLook != null && targeter.Target != null)
                    ((LookSystem)LookSystem.Instance).
                        LookAt(targeter.CachedLook, targeter.Target.position);
            }
        }
    }
}
