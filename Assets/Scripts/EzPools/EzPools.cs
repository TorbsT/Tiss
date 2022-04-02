using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EzPools : MonoBehaviour
{
    public GameObject UIItemPrefab => uiItemPrefab;
    [SerializeField] private GameObject uiItemPrefab;
    public static EzPools Instance { get; private set; }
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
        //Debug.Log(go + " " + go.activeSelf);
        return go;
    }
    /*
    public GameObject Depool(string id)
    {
        Pool pool = GetById(id);
        if (pool == null) Debug.LogWarning("Tried depooling " + id + ", but no such pool exists");
        GameObject go = pool.Depool();
        return go;
    }
    */

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


    public class Pool : ScriptableObject
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
                result.GetComponent<Destroyable>().JustDepooled();
            }
            else
            {
                result = Instantiate(prefab);
                result.name = id + created.ToString();
                Destroyable destroyable = result.GetComponent<Destroyable>();
                if (destroyable != null)
                {
                    destroyable.JustInstantiated(this);
                } else
                {
                    Debug.LogWarning(result + " does not have the Destroyable component");
                }
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
}