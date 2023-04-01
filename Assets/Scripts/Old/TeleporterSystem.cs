using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterSystem : MonoBehaviour
{
    public static TeleporterSystem Instance { get; private set; }

    [SerializeField] private string disappearParticle = "";
    [SerializeField] private string appearParticle = "";
    [SerializeField] private List<Teleporter> teleporters = new List<Teleporter>();
    private void Awake()
    {
        Instance = this;
    }

    public void Teleport(Transform trans, Teleporter teleporter)
    {
        int tries = 0;
        int index = teleporter.Id;
        Teleporter destination = null;
        while (tries < teleporters.Count)
        {
            index++;
            if (index >= teleporters.Count) index = 0;
            if (teleporters[index].Powered)
            {
                destination = teleporters[index];
                break;
            }
        }
        if (destination == null) Debug.LogError("What the fuck");
        else
        {
            ParticleSystem.Particle = disappearParticle;
            ParticleSystem.Pos = trans.position;
            ParticleSystem.Spawn();
            trans.position = destination.transform.position;
            ParticleSystem.Particle = appearParticle;
            ParticleSystem.Pos = trans.position;
            ParticleSystem.Spawn();
        }
    }
    public void Track(Teleporter teleporter)
    {
        teleporters.Add(teleporter);
        teleporter.Id = teleporters.Count-1;
    }
    public void Untrack(Teleporter teleporter)
    {
        int index = teleporter.Id;
        teleporters.RemoveAt(index);
        for (int i = index; i < teleporters.Count; i++)
        {
            teleporters[i].Id = i;
        }
    }
}
