using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorSystem : MonoBehaviour, IEventListener
{
    public static GeneratorSystem Instance { get; private set; }
    public int GlobalPower => globalPower;
    [SerializeField] private int globalPower;
    [SerializeField] private float reducedFuelEachRound;

    private bool currentlyDraining;
    private float reducedEachSecond;
    private bool rememberPowerChanged;
    private HashSet<Generator> generators = new();
    private void Awake()
    {
        Instance = this;
        EventSystem.AddEventListener(this, Event.RotationsDone);
        EventSystem.AddEventListener(this, Event.NewRound);
        EventSystem.AddEventListener(this, Event.NewWave);
    }
    void Update()
    {
        if (rememberPowerChanged)
        {
            EventSystem.DeclareEvent(Event.PowerChanged);
            rememberPowerChanged = false;
        }
        if (currentlyDraining)
        foreach (Generator gen in generators)
        {
            gen.GetComponent<HP>().Decrease(reducedEachSecond * Time.deltaTime);
        }
    }
    public void Track(Generator gen)
    {
        generators.Add(gen);
    }
    public void Untrack(Generator gen)
    {
        generators.Remove(gen);
    }
    public void EventDeclared(Event e)
    {
        if (e == Event.RotationsDone)
        {
            foreach (Generator gen in FindObjectsOfType<Generator>())
            {
                gen.RequestUpdate();
            }
        } else if (e == Event.NewRound)
        {
            currentlyDraining = false;
        } else if (e == Event.NewWave)
        {
            currentlyDraining = true;
            reducedEachSecond = reducedFuelEachRound/RoundSystem.Instance.CurrentWaveDuration;
        }
    }
    public void NotifyPowerChanged()
    {
        rememberPowerChanged = true;
    }

}
