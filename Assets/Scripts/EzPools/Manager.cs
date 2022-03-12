using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EzPools
{
    public class Manager : MonoBehaviour
    {
        public GameObject UIItemPrefab => uiItemPrefab;
        [SerializeField] private GameObject uiItemPrefab;
        public static Manager Instance { get; private set; }
        private readonly Dictionary<string, Pool> dict = new();

        private void Awake()
        {
            Instance = this;
        }

        public GameObject Depool(GameObject prefab)
        {
            string id = prefab.name;
            Pool pool = GetById(id);

            if (pool == null)
            {
                pool = ScriptableObject.CreateInstance<Pool>();
                pool.Prefab = prefab;
                pool.Id = id;
                dict.Add(id, pool);
            }

            GameObject go = pool.Depool();
            Debug.Log("Called depool, returned " + go);
            return go;
        }
        public GameObject Depool(string id)
        {
            Pool pool = GetById(id);
            if (pool == null) Debug.LogWarning("Tried depooling " + id + ", but no such pool exists");
            GameObject go = pool.Depool();
            Debug.Log("Called depool, returned " + go);
            return go;
        }
        public void Enpool(GameObject go)
        {
            Debug.Log("Called enpool on " + go);
            SourcePool source = go.GetComponent<SourcePool>();
            Pool pool;
            if (source == null)
            {
                pool = GetByGO(go);
                if (pool == null)
                {
                    Debug.LogWarning(go + " does not have a source pool - destroying directly");
                    Destroy(go);
                    return;
                } else
                {
                    go.AddComponent<SourcePool>().Pool = pool;
                }
            } else
            {
                pool = source.Pool;
            }
            pool.Enpool(go);
        }

        private Pool GetByGO(GameObject go) => GetById(GetIdByGO(go));
        private string GetIdByGO(GameObject go) => go.name;
        private Pool GetById(string id)
        {
            if (!dict.ContainsKey(id))
            {
                return null;
            }
            return dict[id];
        }


        private class Pool : ScriptableObject
        {
            public GameObject Prefab { get => prefab; set { prefab = value; } }
            public string Id { get => id; set { id = value; } }
            [SerializeField] private GameObject prefab;
            [SerializeField] private string id;
            [SerializeField] private int released;
            [SerializeField] private int pooled;
            [SerializeField] private int created;
            private readonly Queue<GameObject> queue = new();

            public GameObject Depool()
            {
                GameObject result;

                if (queue.Count > 0)
                {
                    result = queue.Dequeue();
                }
                else
                {
                    result = Instantiate(prefab);
                    result.name = id + created.ToString();
                    result.AddComponent<SourcePool>().Pool = this;
                    created++;
                }

                result.SetActive(true);
                return result;
            }
            public void Enpool(GameObject go)
            {
                queue.Enqueue(go);
                go.transform.SetParent(Instance.transform);
                go.SetActive(false);
            }
        }
        private class SourcePool : MonoBehaviour
        {
            public Pool Pool { get => pool; set { pool = value; } }
            [SerializeField] Pool pool;
        }
    }
}