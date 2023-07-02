using Assets.Scripts.Components;
using Assets.Scripts.Systems.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    // not DebuffManager :)
    public abstract class DebuffSystem : MonoBehaviour, IDebuffSystem
    {
        protected ICollection<Debuffable> Components => components.Copy();
        private HashSet<Debuffable> components = new();

        public void Track(Debuffable debuffable)
        {
            components.Add(debuffable);
            Tracked(debuffable);
        }
        public void Untrack(Debuffable debuffable)
        {
            components.Remove(debuffable);
            Untracked(debuffable);
        }

        protected float GetHighestValue(Debuffable component)
        {
            Debuff highest = null;
            foreach (var debuff in component.Debuffs)
                if (debuff.Name == name)
                    if (highest == null || debuff.Value > highest.Value)
                        highest = debuff;
            return highest.Value;
        }
        protected abstract void Tick(ICollection<Debuffable> components);
        protected virtual void Tracked(Debuffable debuffable) { }
        protected virtual void Untracked(Debuffable debuffable) { }
        protected void Update()
        {
            Tick(Components);
        }
    }
}