using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;
using Pathfinding;
using Pools;

public class Zombie : MonoBehaviour, IPathfinderListener, ITargetChooserListener, IHPListener
{
    [Header("CONFIG")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float attackDelay;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackDamage;

    [Header("DEBUG")]
    [SerializeField] private Vector2 goal;
    [SerializeField] private float timeSinceTargetUpdate;
    [SerializeField] private float timeSinceAttack;
    [SerializeField] private Room currentRoom;

    private HP hp;
    private Locomotion locomotion;
    private Transform subgoal;
    private TargetChooser chooser;
    private Target target;
    private Pathfinder pathfinder;
    private IAttackable cachedAttackable;

    private void Awake()
    {
        locomotion = GetComponent<Locomotion>();
        pathfinder = GetComponent<Pathfinder>();
        chooser = GetComponent<TargetChooser>();
        hp = GetComponent<HP>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        PathfindingManager.Instance.AddZombie(this);
        hp.Set(100f);
    }
    private void OnDisable()
    {
        PathfindingManager.Instance.RemoveZombie(this);
    }
    

    // Update is called once per frame
    void Update()
    {
        if (target == null && !chooser.Running)
        {
            Research();
        }

        if (subgoal == this || subgoal == null) locomotion.Speed = 0f;
        else
        {
            locomotion.Direction = subgoal.position.Subtract(transform.position);
            locomotion.Speed = walkSpeed;
        }

        timeSinceAttack += Time.deltaTime;
        if (cachedAttackable != null && timeSinceAttack > attackDelay)
        {
            if ((target.transform.position-transform.position).magnitude < attackRange)
            {
                timeSinceAttack = 0f;
                cachedAttackable.Attack(attackDamage);
            }
        }
    }
    public void Damage(float damage)
    {
        hp.Decrease(damage);
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
        cachedAttackable = target.GetComponent<IAttackable>();
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
