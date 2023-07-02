using Assets.Scripts.ScriptableObjects;
using Assets.Scripts.Systems.Towers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    public class UpgradeSystem : MonoBehaviour
    {
        public static UpgradeSystem Instance { get; private set; }
        private readonly List<IUpgradableSystem> upgradables = new();

        private void Awake()
        {
            Instance = this;
            foreach (var upgradable in 
                GetComponentsInChildren<IUpgradableSystem>())
                upgradables.Add(upgradable);
        }

        public bool TowerExists(string tower)
            => GetTower(tower) != null;
        public void SetBool(string tower, string name, bool value)
            => GetTower(tower).SetBool(name, value);
        public void SetFloat(string tower, string name, float value)
            => GetTower(tower).SetFloat(name, value);
        public void SetInt(string tower, string name, int value)
            => GetTower(tower).SetInt(name, value);
        public bool? GetBool(string tower, string name)
            => GetTower(tower).GetBool(name);
        public float? GetFloat(string tower, string name)
            => GetTower(tower).GetFloat(name);
        public int? GetInt(string tower, string name)
            => GetTower(tower).GetInt(name);
        private IUpgradableSystem GetTower(string name)
        {
            foreach (var upgradeable in upgradables)
                if (upgradeable.gameObject.name == name)
                    return upgradeable;
            return null;
        }
    }
}
