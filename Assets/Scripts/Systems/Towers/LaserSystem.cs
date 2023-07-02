using Assets.Scripts.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Systems.Towers
{
    public class LaserSystem : QuickSystem<Laser>
    {
        protected override void Tick(ICollection<Laser> lasers)
        {
            foreach (var laser in lasers)
            {
                Vector2 originPos = laser.LaserOrigin.position;

                // Calculate laser
                float maxRange = 100f;
                Vector2 endPos;
                RaycastHit2D hitData = Physics2D.Raycast
                    (originPos, laser.LaserOrigin.right, maxRange,
                    1 << LayerSystem.Instance.WallLayer);
                if (hitData.transform == null)
                    endPos = originPos +
                        (Vector2)laser.LaserOrigin.right * maxRange;
                else
                    endPos = hitData.point;
                float distance = (endPos - originPos).magnitude;

                // Show visual
                Vector2 laserVisualOffset = (endPos-originPos).normalized*laser.LaserVisualShortening;
                laser.LaserVisual.SetPositions
                    (new Vector3[] {
                        (Vector2)laser.LaserVisualOrigin.position+laserVisualOffset,
                        endPos-laserVisualOffset });
                laser.OriginPos = originPos;
                laser.ImpactPos = endPos;

                // Adjust hitbox
                Vector2 center = (endPos-originPos) / 2f;
                Vector3 size = new(distance, laser.Width, 1f);
                laser.Collider.size = size;
                laser.Collider.offset = Vector2.right * distance / 2f;
                laser.transform.localPosition = Vector2.zero;

                // Deal damage
                foreach (var enemy in laser.CurrentCollisions)
                {
                    if (enemy.isActiveAndEnabled)
                    ((EnemySystem)EnemySystem.Instance)
                        .Damage(enemy, laser.DPS*Time.deltaTime);
                }
                laser.CurrentCollisions = new();
            }
        }
    }
}