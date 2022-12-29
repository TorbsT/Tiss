using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ZombieBrainSystem : MonoBehaviour
{
    public struct SourceDestination
    {
        Vector2Int source;
        Vector2Int destination;
        public SourceDestination(Vector2Int source, Vector2Int destination)
        {
            this.source = source;
            this.destination = destination;
        }
    }
    public static ZombieBrainSystem Instance { get; private set; }
    public Dictionary<SourceDestination, Vector2Int> planSubgoalCache = new();
    public HashSet<Zombie> unplanned = new();
    public HashSet<Zombie> planned = new();
    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        HashSet<Zombie> tempUnplanned = new();
        foreach (Zombie zombie in unplanned)
        {
            tempUnplanned.Add(zombie);
        }

        foreach (Zombie zombie in tempUnplanned)
        {
            //how should zombies choose and navigate?
            zombie.Target = Player.Instance.GetComponent<Target>();
            zombie.Subgoal = zombie.Target.transform;
            unplanned.Remove(zombie);
        }
    }
    public void ZombieSpawned(Zombie zombie)
    {
        unplanned.Add(zombie);
    }
    public void ZombieDespawned(Zombie zombie)
    {
        unplanned.Remove(zombie);
        planned.Remove(zombie);
    }
}
