using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;

public class Locomotion : MonoBehaviour
{
    public float BaseSpeed { get => baseSpeed; set { baseSpeed = value; RecalculateSpeed(); } }
    public float Speed => speed;
    public Vector2 Direction { get => direction; set { direction = value; } }

    [Header("ASSIGN")]
    [SerializeField] private float baseSpeed;

    [Header("DEBUG")]
    [SerializeField] private float speed;
    [SerializeField] private Vector2 direction;
    [SerializeField] private Vector2 delta;
    private HashSet<Splurge> splurges = new();


    void FixedUpdate()
    {
        delta = direction.normalized * speed * Time.fixedDeltaTime;
        transform.position = transform.position.Add(delta);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Splurge s = collision.GetComponent<Splurge>();
        if (s != null)
        {
            splurges.Add(s);
            RecalculateSpeed();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Splurge s = collision.GetComponent<Splurge>();
        if (s != null)
        {
            splurges.Remove(s);
            RecalculateSpeed();
        }
        
    }


    private void RecalculateSpeed()
    {
        speed = baseSpeed;
        float splurgeMod = 1f;
        foreach (Splurge splurge in splurges)
        {
            splurgeMod = Mathf.Min(splurge.Amount, splurgeMod);
        }
        speed *= splurgeMod;
    }
}
