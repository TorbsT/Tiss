using Assets.Scripts.Systems.Common;
using Assets.Scripts.Systems.Towers;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    public class OtherUpgradesSystem : MonoBehaviour, IUpgradableSystem
    {
        private Upgrades upgrades = new();
        private bool queueRefresh = false;

        public bool? GetBool(string name)
            => upgrades.GetBool(name);
        public float? GetFloat(string name)
            => upgrades.GetFloat(name);
        public int? GetInt(string name)
            => upgrades.GetInt(name);

        public void SetBool(string name, bool value)
        {
            upgrades.SetBool(name, value);
            queueRefresh = true;
        }
        public void SetFloat(string name, float value)
        {
            upgrades.SetFloat(name, value);
            queueRefresh = true;
        }
        public void SetInt(string name, int value)
        {
            upgrades.SetInt(name, value);
            queueRefresh = true;
        }

        private void OnEnable()
        {
            SetInt("lootcount", 100);
            SetFloat("lootrarity", 1f);
            SetFloat("enemyspawns", 1f);
            //SetFloat("enemyhealth", 1f); TODO
        }
        private void Update()
        {
            if (queueRefresh)
            {
                Refresh();
                queueRefresh = false;
            }
        }
        private void Refresh()
        {
            ChestSystem.Instance.LootCount = GetInt("lootcount").Value;
            ChestSystem.Instance.LootRarity = GetFloat("lootrarity").Value;
            WaveSystem waveSystem = WaveSystem.Instance;
            waveSystem.SpawnRate = GetFloat("enemyspawns").Value;
        }
    }
}