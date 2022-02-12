using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float walkSpeed;
    private CharacterController controller;

    // Start is called before the first frame update
    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float ws = Input.GetAxis("Vertical");
        float ad = Input.GetAxis("Horizontal");

        Vector3 movement = new(ad*walkSpeed, 0f, ws*walkSpeed);
        controller.Move(movement);
    }
}
