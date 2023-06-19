using UnityEngine;
using TorbuTils.EzPools;
using System;

namespace Assets.Scripts.Systems
{
    public class PoolSystem : MonoBehaviour
    {
        public event Action<GameObject> JustDepooled;
        public event Action<GameObject> JustEnpooled;

        public static PoolSystem Instance { get; private set; }

        public static GameObject Depool(GameObject prefab)
        {
            GameObject go = Pools.Instance.Depool(prefab);
            RecursiveEvent(go.transform, obj => Instance.JustDepooled?.Invoke(obj));
            return go;
        }
        public static void Enpool(GameObject go)
        {
            RecursiveEvent(go.transform, obj => Instance.JustEnpooled?.Invoke(obj));
            Pools.Instance.Enpool(go);
        }

        void Awake()
        {
            Instance = this;
        }
        private static void RecursiveEvent(Transform transform, Action<GameObject> action)
        {
            if (transform.childCount > 0)
                foreach (var child in transform.GetComponentsInChildren<Transform>())
                    if (child.parent == transform)
                        RecursiveEvent(child, action);
            action.Invoke(transform.gameObject);
        }
    }
}