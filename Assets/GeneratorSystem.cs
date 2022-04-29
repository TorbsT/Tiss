using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorSystem : MonoBehaviour, IEventListener
{
    public static GeneratorSystem Instance { get; private set; }
    public int GlobalPower => globalPower;
    [SerializeField] private int globalPower;

    private bool rememberPowerChanged;
    private void Awake()
    {
        Instance = this;
        EventSystem.AddEventListener(this, Event.RotationsDone);
    }
    void Update()
    {
        if (rememberPowerChanged)
        {
            EventSystem.DeclareEvent(Event.PowerChanged);
            rememberPowerChanged = false;
        }
    }
    public void EventDeclared(Event e)
    {
        if (e == Event.RotationsDone)
        {
            foreach (Generator gen in FindObjectsOfType<Generator>())
            {
                gen.RequestUpdate();
            }
        }
    }
    public void NotifyPowerChanged()
    {
        rememberPowerChanged = true;
    }

}
