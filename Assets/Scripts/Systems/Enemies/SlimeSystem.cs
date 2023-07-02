using Assets.Scripts.Components.Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Systems.Enemies
{
    public class SlimeSystem : QuickSystem<Slime>
    {
        [field: SerializeField] public float MotherMaxHealth
        { get; set; } = 700f;
        [field: SerializeField] public float MotherSpeed
        { get; set; } = 0.2f;
        [field: SerializeField] public float SplitForce
        { get; set; } = 0.2f;
        [field: SerializeField] public float MinMaxHealthToDivide
        { get; set; } = 1f;
        [field: SerializeField, Range(2, 10)] public int DeathDivides
        { get; set; } = 2;
        protected override void Tick(ICollection<Slime> slimes)
        {
            foreach (var slime in slimes)
            {
                float maxHealth = slime.CachedEnemy.MaxHealth;
                float health = slime.CachedEnemy.Health;
                if (maxHealth < MinMaxHealthToDivide) continue;
                if (health > maxHealth / 2f) continue;
                Vector2 childPos = slime.transform.position;
                
                for (int i = 0; i < DeathDivides; i++)
                {
                    Slime child = ((EnemySystem)EnemySystem.Instance).Summon(childPos, "slime")
                        .GetComponent<Slime>();
                    float childMaxHealth = health;
                    SetSize(child, childMaxHealth);
                    child.CachedRb.velocity = Random.insideUnitCircle * SplitForce;
                }
                ((EnemySystem)EnemySystem.Instance).Kill(slime);
                //PoolSystem.Enpool(slime.gameObject);
            }
        }
        protected override void JustDepooledComponent(Slime component)
        {
            SetSize(component, MotherMaxHealth);
        }
        private void SetSize(Slime slime, float health)
        {
            slime.CachedEnemy.MaxHealth = health;
            slime.CachedEnemy.Health = health;
            slime.transform.localScale = Vector3.one * health / MotherMaxHealth;

            slime.CachedLocomotion.AllMultiplier =
                Mathf.Lerp(MotherSpeed, 1f, 1f - health / MotherMaxHealth);
        }
    }
}