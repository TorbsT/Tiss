using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour, IPowerListener, IRoomDwellerListener
{
    [SerializeField] private int power;
    [SerializeField] private GradientObject powerGradient;
    [SerializeField] private new SpriteRenderer renderer;
    private HashSet<IBatteryListener> listeners = new();

    void Awake()
    {
        foreach (IBatteryListener listener in GetComponents<IBatteryListener>())
        {
            AddListener(listener);
        }
    }
    public void NewPower(int power)
    {
        if (renderer != null && powerGradient != null)
            renderer.color = powerGradient.Gradient.Evaluate((float)power / RoomManager.Instance.GeneratorPower);
        foreach (IBatteryListener listener in listeners)
        {
            listener.NewCharge(this.power, power);
        }
        this.power = power;
    }
    public void AddListener(IBatteryListener listener)
    {
        listeners.Add(listener);
        listener.NewCharge(power, power);
    }
    public void RoomChanged(Room oldRoom, Room newRoom)
    {
        if (oldRoom != null)
        oldRoom.RemovePowerListener(this);
        if (newRoom != null)
        newRoom.AddPowerListener(this);
    }
}
