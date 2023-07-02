using Assets.Scripts.Components;
using Assets.Scripts.Components.Towers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Systems.Towers
{
    public class BaseSystem : TowerSystem<Base>
    {
        [SerializeField] private float maxHp = 1000;
        protected override void JustEnabled()
        {
            SetFloat("maxhp", maxHp);
            SetFloat("hp", maxHp);  // One-time healing to base
            SetFloat("tacticalhp", 0);  // TODO after rounds, heal if upgraded
        }
        protected override void Tick(ICollection<Base> components)
        {
            float maxHp = GetFloat("maxhp").Value;
            float hp = GetFloat("hp").Value;

            foreach (var b in components)
            {
                foreach (var enemy in b.CollisionEnters)
                {
                    b.Hp -= enemy.GetComponent<Enemy>().Health;
                    ((EnemySystem)EnemySystem.Instance).Kill(enemy);
                    if (b.Hp <= 0)
                    {
                        PoolSystem.Enpool(b.gameObject);
                        break;
                    }
                }
                b.CollisionEnters = new();

                b.MaxHp = maxHp;
                b.Hp += hp;
                if (b.Hp > maxHp)
                    b.Hp = maxHp;
            }
            SetFloat("hp", 0f);
        }
    }
}