using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;
using Pathfinding;
using Pools;

public class Zombie : MonoBehaviour, IPathfinderListener, ITargetChooserListener, IHPListener
{
    [Header("CONFIG")]
    [SerializeField] private float attackDelay;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackDamage;

    [Header("DEBUG")]
    [SerializeField] private bool gizmos;
    [SerializeField] private Vector2 goal;
    [SerializeField] private float timeSinceTargetUpdate;
    [SerializeField] private float timeSinceAttack;
    [SerializeField] private Room currentRoom;

    private Locomotion locomotion;
    private Transform subgoal;
    private TargetChooser chooser;
    private Target target;
    private Pathfinder pathfinder;
    private HashSet<Transform> collisions = new();
    private LookAt lookat;

    private void Awake()
    {
        locomotion = GetComponent<Locomotion>();
        pathfinder = GetComponent<Pathfinder>();
        chooser = GetComponent<TargetChooser>();
        lookat = GetComponent<LookAt>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<HP>().AddListener(this);
    }
    void OnDrawGizmosSelected()
    {
        if (!gizmos) return;
        Gizmos.color = Color.red;
        foreach (Transform t in collisions)
        {
            Gizmos.DrawSphere(t.position, 1f);
        }
    }
    private void OnEnable()
    {
        PathfindingManager.Instance.AddZombie(this);
        collisions = new();
        GetComponent<HP>().Set(100f);
    }
    private void OnDisable()
    {
        PathfindingManager.Instance.RemoveZombie(this);
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        collisions.Add(collision.transform);
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        collisions.Remove(collision.transform);
    }
    // Update is called once per frame
    void Update()
    {
        if (target == null && !chooser.Running)
        {
            Research();
        }

        if (subgoal != this && subgoal != null)
        {
            locomotion.Direction = subgoal.position.Subtract(transform.position);
            lookat.LookingAt = subgoal.position;
        }

        timeSinceAttack += Time.deltaTime;
        if (timeSinceAttack > attackDelay)
        {
            foreach (Transform t in collisions)
            {
                Team otherTeam = t.GetComponent<Team>();
                if (otherTeam != null && GetComponent<Team>().CanInjure(otherTeam))
                {
                    HP hp = otherTeam.GetComponent<HP>();
                    if (hp != null)
                    {
                        timeSinceAttack = 0f;
                        hp.Decrease(attackDamage);
                        break;
                    }
                }
            }

        }
    }
    public void Despawn()
    {
        ZombiePool.Instance.Enpool(this);
    }
    public void Research()
    {
        chooser.Research();
    }
    public void ChoseNewTarget(Target current, Target newTarget)
    {
        pathfinder.SetTarget(newTarget);
        target = newTarget;
    }
    public void NewSubgoal(Transform old, Transform newGoal)
    {
        subgoal = newGoal;
    }

    public void NewHP(float oldHP, float newHP)
    {
        if (newHP <= 0f) Despawn();
    }
}
