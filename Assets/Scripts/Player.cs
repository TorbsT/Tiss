using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float walkSpeed;

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float ws = Input.GetAxis("Vertical");
        float ad = Input.GetAxis("Horizontal");

        Vector2 movement = new(ad*walkSpeed, ws*walkSpeed);
        transform.position = new Vector2(transform.position.x+movement.x, transform.position.y+movement.y);
    }
}
