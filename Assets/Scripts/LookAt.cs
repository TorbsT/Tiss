using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Vector2 LookingAt { get => lookingAt; set { lookingAt = value; } }
    private Vector2 lookingAt;

    // cached
    private Rewinder rewinder;


    private void Awake()
    {
        rewinder = GetComponent<Rewinder>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rewinder != null && rewinder.Rewinding) return;
        Vector2 pos = transform.position;
        float angle = Vector2.SignedAngle(Vector2.up, lookingAt - pos);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
