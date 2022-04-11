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
    [SerializeField] private float baseSpeed = 3f;
    [SerializeField] private float acceleration = 100f;
    [SerializeField] private float deceleration = 100f;

    [Header("DEBUG")]
    [SerializeField] private float speed;
    [SerializeField] private Vector2 direction;
    [SerializeField] private Vector2 delta;
    private HashSet<Splurge> splurges = new();
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        RecalculateSpeed();
    }
    void FixedUpdate()
    {
        Vector2 oldVelocity = rb.velocity;
        float oldMagnitude = oldVelocity.magnitude;

        if (direction == Vector2.zero)
        {
            // Decelerate
            if (oldMagnitude == 0f) return;
            float newMagnitude = oldMagnitude - deceleration*Time.fixedDeltaTime;
            newMagnitude = Mathf.Max(0f, newMagnitude);

            rb.velocity *= newMagnitude / oldMagnitude;
        } else
        {
            // Accelerate
            if (acceleration == 0f) return;
            Vector2 newVelocity = oldVelocity + acceleration*direction.normalized*Time.fixedDeltaTime;
            float newMagnitude = newVelocity.magnitude;
            newMagnitude = Mathf.Min(speed, newMagnitude);
            newVelocity *= newMagnitude / newVelocity.magnitude;

            rb.velocity = newVelocity;
        }
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
