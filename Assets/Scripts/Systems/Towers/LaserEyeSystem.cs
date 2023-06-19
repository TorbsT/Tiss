using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Systems.Towers
{
    using Assets.Scripts.Components;
    using Assets.Scripts.Components.Towers;
    internal class LaserEyeSystem : TowerSystem<LaserEye>
    {
        protected override void Tick(ICollection<LaserEye> turrets)
        {
            foreach (var eye in turrets)
            {
                Vector2 originPos = eye.LaserVisual.transform.position;

                // Calculate laser
                float maxRange = 100f;
                Vector2 endPos;
                RaycastHit2D hitData = Physics2D.Raycast
                    (originPos, eye.transform.right, maxRange,
                    1 << LayerSystem.Instance.WallLayer);
                if (hitData.transform == null)
                    endPos = originPos +
                        (Vector2)eye.transform.right * maxRange;
                else
                    endPos = hitData.point;
                float distance = (endPos - originPos).magnitude;

                // Show visual
                eye.LaserVisual.SetPositions
                    (new Vector3[] { originPos, endPos });

                // Deal damage
                RaycastHit2D[] enemiesHit = Physics2D.RaycastAll
                    (eye.transform.position, eye.transform.right, distance,
                    1 << LayerSystem.Instance.EnemyLayer);
                foreach (var enemyHit in enemiesHit)
                {
                    var enemy = enemyHit.transform.GetComponent<Enemy>();
                    if (enemy != null)
                        ((EnemySystem)EnemySystem.Instance).Damage(enemy, eye.LaserDPS*Time.deltaTime);
                }

                Heat(eye, 0.02f * Time.deltaTime);
            }
        }
    }
}
