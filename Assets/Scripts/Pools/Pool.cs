using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pools
{
    public abstract class Pool<T> : MonoBehaviour where T : Component
    {
        public T Prefab => prefab;

        [Header("POOL CONFIG")]
        [SerializeField] protected T prefab;
        [SerializeField] protected bool disableObjectsOnEnpool = true;
        [SerializeField] protected bool enableObjectsOnDepool = true;
        [SerializeField] private int releasedCount;
        [SerializeField] private int pooledCount;

        public static Pool<T> Instance { get { if (instance == null) Debug.LogError("NO INSTANCES OF " + typeof(Pool<T>) + " IN SCENE"); return instance; } }

        private Queue<T> enpooledObjects;
        private static Pool<T> instance;

        void Awake()
        {
            enpooledObjects = new();
            if (instance != null)
            {
                Debug.LogWarning("MULTIPLE INSTANCES OF " + this + " IN SCENE");
            }
            instance = this;
        }


        public T Depool()
        {
            T result;
            if (enpooledObjects.Count > 0)
            {
                result = enpooledObjects.Dequeue();
                pooledCount--;
            } else
            {
                result = Instantiate(prefab, transform);
            }

            releasedCount++;
            JustDepooled(result);
            if (enableObjectsOnDepool) result.gameObject.SetActive(true);
            return result;
        }
        public void Enpool(T t)
        {
            JustEnpooled(t);
            if (disableObjectsOnEnpool) t.gameObject.SetActive(false);
            t.transform.SetParent(transform);
            releasedCount--;
            pooledCount++;
            enpooledObjects.Enqueue(t);
        }

        protected virtual void JustEnpooled(T t)
        {

        }
        protected virtual void JustDepooled(T t)
        {

        }
    }
}

