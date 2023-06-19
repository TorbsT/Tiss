using Assets.Scripts.Systems.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    public abstract class QuickSystem<T> : MonoBehaviour where T : Component
    {
        public static QuickSystem<T> Instance { get; private set; }
        //public event Action<T> JustDepooled;
        //public event Action<T> JustEnpooled;
        private ICollection<T> ActiveComponents { get; } = new HashSet<T>();
        protected Dictionary<GameObject, T> ComponentDict { get; } = new();

        public ICollection<T> CopyComponents()
            => ActiveComponents.Copy();
        protected abstract void Tick(ICollection<T> components);
        protected virtual void FixedTick(ICollection<T> components) { }
        protected virtual void JustEnabled() { }
        protected virtual void JustDisabled() { }
        protected virtual void JustDepooledComponent(T component) { }
        protected virtual void JustEnpooledComponent(T component) { }
        protected void Awake()
        {
            Instance = this;
        }
        protected void OnEnable()
        {
            PoolSystem.Instance.JustDepooled += Depooled;
            PoolSystem.Instance.JustEnpooled += Enpooled;
            foreach (var component in FindObjectsOfType<T>())
                ActiveComponents.Add(component);
            JustEnabled();
        }
        protected void OnDisable()
        {
            PoolSystem.Instance.JustDepooled -= Depooled;
            PoolSystem.Instance.JustEnpooled -= Enpooled;
            JustDisabled();
        }
        protected void Update()
        {
            Tick(ActiveComponents.Copy());
        }
        private void FixedUpdate()
        {
            FixedTick(ActiveComponents.Copy());
        }
        private void Depooled(GameObject go)
        {
            T component = go.GetComponent<T>();
            if (component == null) return;
            ActiveComponents.Add(component);
            ComponentDict.Add(go, component);
            JustDepooledComponent(component);
            //JustDepooled?.Invoke(component);
        }
        private void Enpooled(GameObject go)
        {
            T component = go.GetComponent<T>();
            if (component == null)
                component = go.GetComponentInChildren<T>();
            if (component == null) return;
            ActiveComponents.Remove(component);
            ComponentDict.Remove(go);
            JustEnpooledComponent(component);
            //JustEnpooled?.Invoke(component);
        }
    }
}
