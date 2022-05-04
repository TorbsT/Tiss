using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    [SerializeField] private string onEnable;
    [SerializeField] private string onDisable;

    private void OnEnable()
    {
        Spawn(onEnable);
    }
    private void OnDisable()
    {
        Spawn(onDisable);
    }
    public void Spawn(string particle)
    {
        if (particle == "") return;
        ParticleSystem.Pos = transform.position;
        ParticleSystem.Particle = particle;
        ParticleSystem.Instance.Spawn();
    }
}
