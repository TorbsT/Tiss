using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystem : MonoBehaviour
{
    [System.Serializable]
    private class Pair
    {
        public string name;
        public GameObject prefab;
    }
    public static ParticleSystem Instance { get; private set; }

    public static Vector2 Pos { set { Instance.pos = value; } }
    public static string Particle { set { Instance.particleName = value; } }

    [Header("CONFIG")]
    [SerializeField] private List<Pair> particles;

    [Header("DEBUG")]
    [SerializeField] private Vector2 pos;
    [SerializeField] private string particleName;
    private void Awake()
    {
        Instance = this;
    }

    public void Spawn()
    {
        GameObject prefab = GetPrefab(particleName);
        if (prefab == null)
        {
            Debug.LogWarning("Tried spawning particle " + particleName + ", but it does not exist");
            return;
        }
        GameObject go = EzPools.Instance.Depool(prefab);
        go.transform.position = pos;
    }
    private GameObject GetPrefab(string name)
    {
        foreach (Pair p in particles)
        {
            if (p.name == name) return p.prefab;
        }
        return null;
    }
}
