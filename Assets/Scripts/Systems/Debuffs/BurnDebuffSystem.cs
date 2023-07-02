using Assets.Scripts.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Systems.Debuffs
{
    public class BurnDebuffSystem : DebuffSystem
    {
        private Dictionary<Debuffable, VFXSystem.Effect> vfxs = new();
        protected override void Tick(ICollection<Debuffable> components)
        {
            foreach (var component in components)
            {
                float value = GetHighestValue(component);
                float damage = Time.deltaTime * value;
                EnemySystem.Instance.Damage(component, damage);
            }
        }
        protected override void Tracked(Debuffable debuffable)
        {
            var vfx = debuffable.GetVFX("burn");
            vfxs.Add(debuffable,
                VFXSystem.Instance.Play(vfx, debuffable.transform));
        }
        protected override void Untracked(Debuffable debuffable)
        {
            if (vfxs.ContainsKey(debuffable))
            {
                VFXSystem.Instance.Stop(vfxs[debuffable]);
                vfxs.Remove(debuffable);
            }
        }
    }
}