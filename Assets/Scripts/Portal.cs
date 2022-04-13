using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public int ZombiesToSpawn { get => zombiesToSpawn; set { zombiesToSpawn = value; } }

    [SerializeField] private int zombiesToSpawn;
    [SerializeField] private bool spawning;
    [SerializeField] private GameObject zombiePrefab;

    private void OnEnable()
    {
        PortalSystem.Instance.Add(this);
    }
    private void OnDisable()
    {
        PortalSystem.Instance.Remove(this);
    }
    private void Update()
    {
        if (!spawning) return;
        if (zombiesToSpawn <= 0)
        {
            spawning = false;
            return;
        }

        GameObject go = EzPools.Instance.Depool(zombiePrefab);
        go.transform.position = transform.position;
        zombiesToSpawn--;
    }
    public void StartSpawning()
    {
        spawning = true;
    }

}
