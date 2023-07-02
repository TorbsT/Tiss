using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

namespace Assets.Scripts.Components
{
    public class Debuffable : MonoBehaviour
    {
        [Serializable]
        private class VFX
        {
            [field: SerializeField] public string Name { get; private set; }
            [field: SerializeField] public VisualEffect Effect { get; private set; }
        }

        [field: SerializeField] private List<VFX> VFXs { get; set; } = new();
        [field: SerializeField] public List<Debuff> Debuffs { get; set; } = new();

        public VisualEffect GetVFX(string name)
        {
            foreach (var vfx in VFXs)
                if (vfx.Name == name)
                    return vfx.Effect;
            Debug.LogWarning($"{this} does not have VFX named {name}");
            return null;
        }
    }
    public class Debuff
    {
        [field: SerializeField] public string Name { get; } = "Unnamed";
        [field: SerializeField] public float Duration { get; set; } = -1;
        [field: SerializeField] public float Value { get; set; } = -1;
        public Debuff(string debuffName, float duration, float value)
        {
            Name = debuffName;
            Duration = duration;
            Value = value;
        }
    }
}
