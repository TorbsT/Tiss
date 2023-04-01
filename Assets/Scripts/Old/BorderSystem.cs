using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderSystem : MonoBehaviour
{
    public static BorderSystem Instance { get; private set; }
    private HashSet<Border> activeBorders = new();
    private void Awake()
    {
        Instance = this;
    }
    public void Add(Border b)
    {
        activeBorders.Add(b);
    }
    public void Remove(Border b)
    {
        activeBorders.Remove(b);
    }
    public Border GetClosest(Vector2 pos)
    {
        Border closest = null;
        float smallestDistance = Mathf.Infinity;
        foreach (Border b in activeBorders)
        {
            Vector2 p = b.transform.position;
            float d = (p - pos).sqrMagnitude;
            if (d < smallestDistance)
            {
                smallestDistance = d;
                closest = b;
            }
        }
        if (closest == null)
        {
            Debug.LogError("This is impossible");
        }
        return closest;
    }

}
