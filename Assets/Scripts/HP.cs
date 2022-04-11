using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP : MonoBehaviour
{
    [System.Serializable]
    private class HPSpritePair
    {
        public float minHP;
        public Sprite sprite;
    }

    [SerializeField] private float health;
    [SerializeField] private SpriteRenderer healthRenderer;
    [SerializeField] private List<HPSpritePair> healthMapper;

    private HashSet<IHPListener> listeners = new();  // outside of own gameobject

    private void Start()
    {
        foreach (IHPListener listener in GetComponents<IHPListener>())
        {
            AddListener(listener);
        }
    }
    private void OnEnable()
    {
        listeners = new();
    }
    public void Set(float newHealth)
    {
        // Inform arbitrary listeners
        foreach (IHPListener l in listeners)
        {
            l.NewHP(health, newHealth);
        }

        if (healthRenderer != null)
        for (int i = healthMapper.Count - 1; i >= 0; i--)
        {
            if (newHealth >= healthMapper[i].minHP)
            {
                healthRenderer.sprite = healthMapper[i].sprite;
                break;
            }
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
        listener.NewHP(health, health);
    }
    private void Modify(float amount)
    {
        Set(health + amount);
    }
}
