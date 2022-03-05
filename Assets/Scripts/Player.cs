using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    [SerializeField] private float walkSpeed;
    private Locomotion locomotion;
    private Target target;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        locomotion = GetComponent<Locomotion>();
        target = GetComponent<Target>();
    }

    private void Start()
    {
        target.SetDiscoverability(Target.Discoverability.discoverable);
    }

    // Update is called once per frame
    void Update()
    {
        float ws = Input.GetAxis("Vertical");
        float ad = Input.GetAxis("Horizontal");

        Vector2 direction = new(ad, ws);
        locomotion.Direction = direction;
        locomotion.Speed = walkSpeed;
    }
}
