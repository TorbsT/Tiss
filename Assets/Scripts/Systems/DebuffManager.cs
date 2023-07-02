using Assets.Scripts.Components;
using Assets.Scripts.Systems.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    // not DebuffSystem :)
    public class DebuffManager : QuickSystem<Debuffable>
    {
        public static new DebuffManager Instance { get; private set; }
        private HashSet<IDebuffSystem> debuffSystems = new();
        
        public void Apply(Component component, string debuffName, float duration, float value)
        {
            Debuffable debuffable = component.GetComponent<Debuffable>();
            if (debuffable == null) return;
            var anyLevelDebuff = GetDebuff(debuffable, debuffName, null);
            var debuff = GetDebuff(debuffable, debuffName, value);
            if (debuff == null)
            {
                if (duration <= 0) return;
                debuffable.Debuffs.Add(new(debuffName, duration, value));
                if (anyLevelDebuff == null)
                    GetSystem(debuffName).Track(debuffable);
                return;
            }
            debuff.Duration += duration;
            if (debuff.Duration <= 0)
            {
                debuffable.Debuffs.Remove(debuff);
                GetSystem(debuffName).Untrack(debuffable);
            }
        }
        protected override void JustEnabled()
        {
            Instance = this;
            foreach (var debuffSystem in FindObjectsOfType<MonoBehaviour>().OfType<IDebuffSystem>())
            {
                debuffSystems.Add(debuffSystem);
            }
        }
        protected override void JustEnpooledComponent(Debuffable component)
        {
            foreach (var sys in debuffSystems)
                sys.Untrack(component);
            component.Debuffs = new();
        }
        protected override void Tick(ICollection<Debuffable> components)
        {
            // TODO if needed, add 0.1s delay between each run
            foreach (var component in components)
            {
                foreach (var debuff in component.Debuffs.Copy())
                Apply(component, debuff.Name, -Time.deltaTime, debuff.Value);
            }
        }
        private Debuff GetDebuff(Debuffable debuffable, string name, float? value)
        {
            foreach (var debuff in debuffable.Debuffs)
                if (debuff.Name == name)
                    if (!value.HasValue || debuff.Value == value)
                        return debuff;
            return null;
        }
        private IDebuffSystem GetSystem(string name)
        {
            foreach (var sys in debuffSystems)
                if (sys.name == name)
                    return sys;
            Debug.LogWarning($"There is no debuff system with name {name}");
            return default;
        }
    }
    public interface IDebuffSystem
    {
        string name { get; }
        void Track(Debuffable debuffable);
        void Untrack(Debuffable debuffable);
    }
}