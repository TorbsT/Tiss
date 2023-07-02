using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Systems.Towers
{
    using Assets.Scripts.Systems;
    using Assets.Scripts.Systems.Common;
    using Extensions;
    public abstract class TowerSystem<T> : QuickSystem<T>, IUpgradableSystem where T : Component
    {
        private Upgrades Upgrades { get; set; } = new();

        public bool? GetBool(string name)
            => Upgrades.GetBool(name);
        public float? GetFloat(string name)
            => Upgrades.GetFloat(name);
        public int? GetInt(string name)
            => Upgrades.GetInt(name);
        public void SetBool(string name, bool value)
        { Upgrades.SetBool(name, value); UpgradesChanged(); }
        public void SetFloat(string name, float value)
        { Upgrades.SetFloat(name, value); UpgradesChanged(); }
        public void SetInt(string name, int value)
        { Upgrades.SetInt(name, value); UpgradesChanged(); }

        protected void Heat(T turret, float amount, float? falloff = null, int? range = null)
        {
            HeatSpreadSystem s = HeatSpreadSystem.Instance;
            if (s == null) return;
            Vector2Int loc = turret.GetLoc();
            s.Heat(loc, amount, falloff, range);
        }
        protected virtual void UpgradesChanged() { }
    }
    public interface IUpgradableSystem
    {
        public GameObject gameObject { get; }
        bool? GetBool(string name);
        void SetBool(string name, bool value);
        float? GetFloat(string name);
        void SetFloat(string name, float value);
        int? GetInt(string name);
        void SetInt(string name, int value);
    }
}