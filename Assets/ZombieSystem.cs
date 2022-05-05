using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSystem : MonoBehaviour, IEventListener
{
    public static ZombieSystem Instance { get; private set; }
    private HashSet<Zombie> zombies = new();
    private bool inWave;

    void Awake()
    {
        Instance = this;
        EventSystem.AddEventListener(this, Event.NewRound);
        EventSystem.AddEventListener(this, Event.NewWave);
    }
    public void Track(Zombie zombie)
    {
        zombies.Add(zombie);
    }
    public void Untrack(Zombie zombie)
    {
        if (zombies.Contains(zombie))
        {
            zombies.Remove(zombie);
            if (inWave && zombies.Count == 0) EventSystem.DeclareEvent(Event.ZombiesKilledDuringWave);
        }
    }
    void IEventListener.EventDeclared(Event e)
    {
        if (e == Event.NewRound)
        {
            inWave = false;
        } else if (e == Event.NewWave)
        {
            inWave = true;
        }
    }
}
