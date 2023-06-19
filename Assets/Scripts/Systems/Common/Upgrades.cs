using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Systems.Common
{
    public class Upgrades
    {
        private readonly Dictionary<string, bool> bools = new();
        private readonly Dictionary<string, float> floats = new();
        private readonly Dictionary<string, int> ints = new();
        public void SetBool(string name, bool value)
        {
            bools[name] = value;
        }
        public bool? GetBool(string name)
        {
            if (bools.ContainsKey(name))
                return bools[name];
            return null;
        }
        public void SetFloat(string name, float value)
        {
            floats[name] = value;
        }
        public float? GetFloat(string name)
        {
            if (floats.ContainsKey(name))
                return floats[name];
            return null;
        }
        public void SetInt(string name, int value)
        {
            ints[name] = value;
        }
        public int? GetInt(string name)
        {
            if (ints.ContainsKey(name))
                return ints[name];
            return null;
        }
    }
}
