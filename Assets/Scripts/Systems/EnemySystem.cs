using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    using Extensions;
    using Components;
    using TorbuTils.Giraphe;
    internal class EnemySystem : QuickSystem<Enemy>
    {
        public static new EnemySystem Instance { get; private set; }
        [field: SerializeField] public List<Enemy> EnemyPrefabs { get; private set; }
        private int instanceIdCounter = 0;
        private Dictionary<int, Enemy> instances = new();

        internal void Kill(Component component)
        {
            Damage(component, component.GetComponent<Enemy>().Health);
        }
        internal void Damage(Component component, float amount)
        {
            Enemy enemy = component.GetComponent<Enemy>();
            if (enemy == null) return;
            enemy.Health -= amount;
            if (enemy.Health <= 0)
            {
                CurrencySystem.Instance.Balance += enemy.Bounty;
                PoolSystem.Enpool(enemy.gameObject);
            }
        }
        public Enemy Summon(Vector2 pos, string name = null)
        {
            if (name == null)
                name = EnemyPrefabs
                    [UnityEngine.Random.Range(0, EnemyPrefabs.Count)]
                    .gameObject.name;
            GameObject go = PoolSystem.Depool
                (EnemyPrefabs.Find(match => match.gameObject.name == name).gameObject);
            Enemy enemy = go.GetComponent<Enemy>();
            enemy.Health = enemy.MaxHealth;
            go.transform.position = pos+0.02f * UnityEngine.Random.insideUnitCircle;
            float angle = UnityEngine.Random.Range(0f, 360f);
            go.transform.localRotation = Quaternion.Euler(new(0f, 0f, angle));
            return enemy;
        }
        internal bool IsAlive(int instanceId)
            => GetByInstance(instanceId) != null ?
            GetByInstance(instanceId).gameObject.activeInHierarchy : false;
        internal Enemy GetByInstance(int instanceId)
        {
            if (!instances.ContainsKey(instanceId))
                return null;
            return instances[instanceId];
        }
        protected override void JustEnabled()
        {
            Instance = this;
        }
        protected override void JustDepooledComponent(Enemy enemy)
        {
            instances.Add(instanceIdCounter, enemy);
            enemy.InstanceId = instanceIdCounter;
            instanceIdCounter++;
        }
        protected override void JustEnpooledComponent(Enemy enemy)
        {
            if (instances.ContainsKey(enemy.InstanceId))
                instances.Remove(enemy.InstanceId);
            //Debug.Log($"enpool: {ComponentDict.Count} {enemy.InstanceId}");
        }
        protected override void Tick(ICollection<Enemy> components)
        {
            foreach (var enemy in CopyComponents())
            {
                Vector2Int loc = enemy.GetLoc();
                Vector2Int best = loc;
                Graph<Vector2Int> graph = EnemyNavigation.Instance.Graph;
                foreach (var option in graph.CopyEdgesTo(loc))
                {
                    int? bestScore = (int?)graph.GetSatellite(best, Settings.CostSatellite);
                    int? optionScore = (int?)graph.GetSatellite(option, Settings.CostSatellite);
                    if (optionScore == null) continue;
                    if
                        (bestScore == null || optionScore.Value < bestScore.Value)
                        best = option;
                }
                Vector2 pos = enemy.transform.position;
                Vector2 direction = best - pos;

                // the following is fuckin stupid
                LocomotionSystem locoS = (LocomotionSystem)LocomotionSystem.Instance;
                locoS.Move(enemy.gameObject, direction);
                LookSystem lookS = (LookSystem)LookSystem.Instance;
                lookS.LookAt(enemy, direction);
            }
        }
    }
}