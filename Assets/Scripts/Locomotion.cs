using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;

public class Locomotion : MonoBehaviour
{
    public float Speed { get => speed; set { speed = value; } }
    public Vector2 Direction { get => direction; set { direction = value; } }
    [SerializeField] private float speed;
    [SerializeField] private Vector2 direction;
    [SerializeField] private Vector2 delta;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        delta = direction.normalized * speed*Time.fixedDeltaTime;
        transform.position = transform.position.Add(delta);
    }
}
