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
    [SerializeField] private bool debug;
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

        float standstillMagnitude = oldMagnitude - deceleration * Time.fixedDeltaTime;
        standstillMagnitude = Mathf.Max(0f, standstillMagnitude);

        if (direction == Vector2.zero)
        {
            // Decelerate
            if (oldMagnitude == 0f) return;
            rb.velocity *= standstillMagnitude / oldMagnitude;
        } else
        {
            // Accelerate
            if (acceleration == 0f) return;

            Vector2 walkVelocity = oldVelocity + acceleration*direction.normalized*Time.fixedDeltaTime;
            float walkMagnitude = walkVelocity.magnitude;

            if (walkMagnitude < standstillMagnitude)
            {
                if (debug) Debug.Log("11");
                // Walking direction slows down more than standing still
                walkVelocity *= walkMagnitude / walkVelocity.magnitude;
                rb.velocity = walkVelocity;
            } else
            {
                if (walkMagnitude > speed)
                {
                    if (debug) Debug.Log("22");
                    Vector2 newVelocity = walkVelocity.normalized * (oldMagnitude - deceleration);
                    if (newVelocity.magnitude < speed) newVelocity = newVelocity.normalized * speed;
                    rb.velocity = newVelocity;
                } else
                {
                    if (debug) Debug.Log("33");
                    walkVelocity *= walkMagnitude / walkVelocity.magnitude;
                    rb.velocity = walkVelocity;
                }
            }

            walkMagnitude = Mathf.Min(speed, walkMagnitude);
            walkVelocity *= walkMagnitude / walkVelocity.magnitude;

            rb.velocity = walkVelocity;
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
