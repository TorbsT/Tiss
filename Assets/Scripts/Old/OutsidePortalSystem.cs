using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutsidePortalSystem : MonoBehaviour
{
    public HashSet<Portal> portals = new();
    public GameObject prefab;
    public List<Vector2> spawnPositions;

    public void Spawn()
    {
        foreach (Portal p in portals)
        {
            p.ZombiesToSpawn += 10;
            p.StartSpawning();
        }
    }

    private void OnEnable()
    {
        foreach (Vector2 v2 in spawnPositions)
        {
            Portal p = EzPools.Instance.Depool(prefab).GetComponent<Portal>();
            p.transform.position = v2;
            portals.Add(p);
        }
    }
    private void OnDisable()
    {
        foreach (Portal p in portals)
        {
            p.GetComponent<Destroyable>().Destroy();
        }
        portals.Clear();
    }
}
