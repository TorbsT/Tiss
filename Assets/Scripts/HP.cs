using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP : MonoBehaviour
{
    [SerializeField] private float health;

    private HashSet<IHPListener> listeners = new();  // outside of own gameobject


    private void OnEnable()
    {
        listeners = new();
    }
    public void Set(float newHealth)
    {
        // Inform this GO that HP changed
        IHPListener thisListener = GetComponent<IHPListener>();
        if (thisListener != null) thisListener.NewHP(health, newHealth);

        // Inform arbitrary listeners
        foreach (IHPListener l in listeners)
        {
            l.NewHP(health, newHealth);
        }

        health = newHealth;
    }
    public void Increase(float amount)
    {
        Modify(amount);
    }
    public void Decrease(float amount)
    {
        Modify(-amount);
    }
    public void AddListener(IHPListener listener)
    {
        listeners.Add(listener);
    }
    private void Modify(float amount)
    {
        Set(health + amount);
    }
}
