using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;
using Pathfinding;
using Pools;

public class Zombie : MonoBehaviour, IPathfinderListener, ITargetChooserListener, IHPListener, IBatteryListener
{
    [Header("CONFIG")]
    [SerializeField] private int moneysOnKill = 1;
    [SerializeField] private float lightAttackDelay;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackDamage;
    [SerializeField] private float lightSpeed;
    [SerializeField] private float darkAttackDelay;
    [SerializeField] private float darkSpeed;
    [SerializeField] private float lightAttackKnockback;
    [SerializeField] private float darkAttackKnockback;
    [SerializeField] private float lightMass;
    [SerializeField] private float darkMass;
    [SerializeField] private Sprite darkSprite;
    [SerializeField] private Sprite darkEyesSprite;
    [SerializeField] private SpriteRenderer eyesRenderer;
    [SerializeField, Range(1f, 2f)] private float darkSize = 2f;

    [Header("DEBUG")]
    [SerializeField] private bool gizmos;
    [SerializeField] private bool dark;
    [SerializeField] private Vector2 goal;
    [SerializeField] private float timeSinceTargetUpdate;
    [SerializeField] private float timeSinceAttack;
    [SerializeField] private float attackDelay;
    [SerializeField] private float attackKnockback;
    [SerializeField] private Room currentRoom;

    private Sprite lightSprite;
    private Sprite lightEyesSprite;
    private new SpriteRenderer renderer;
    private Locomotion locomotion;
    public Transform Subgoal { get => subgoal; set { subgoal = value; } }
    public Target Target { get => Target; set { target = value; } }
    private Transform subgoal;
    private Target target;
    private TargetChooser chooser;
    private Pathfinder pathfinder;
    private HashSet<Transform> collisions = new();
    private LookAt lookat;

    private void Awake()
    {
        locomotion = GetComponent<Locomotion>();
        pathfinder = GetComponent<Pathfinder>();
        chooser = GetComponent<TargetChooser>();
        lookat = GetComponent<LookAt>();
        renderer = GetComponent<SpriteRenderer>();
        lightSprite = renderer.sprite;
        lightEyesSprite = eyesRenderer.sprite;
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
        //PathfindingSystem.Instance.AddZombie(this);
        //ZombieSystem.Instance.Track(this);
        ZombieBrainSystem.Instance.ZombieSpawned(this);
        collisions = new();
        GetComponent<HP>().Set(100f);
    }
    private void OnDisable()
    {
        //PathfindingSystem.Instance.RemoveZombie(this);
        //ZombieSystem.Instance.Untrack(this);
        ZombieBrainSystem.Instance.ZombieDespawned(this);
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
                        float dmg = attackDamage;
                        if (t.GetComponent<Player>() != null) dmg *= 10f;
                        hp.Decrease(dmg);

                        Vector2 f = (t.position - transform.position) * attackKnockback;
                        GetComponent<Rigidbody2D>().AddForce(-f);
                        Rigidbody2D rb = t.GetComponent<Rigidbody2D>();
                        if (rb != null)
                        {
                            rb.AddForce(f);
                        }
                        break;
                    }
                }
            }

        }
    }
    public void Despawn()
    {
        GetComponent<Destroyable>().Destroy();
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
        if (newHP <= 0f)
        {
            Player.Instance.GetComponent<IWalletProvider>().Wallet.Shitcoin += moneysOnKill;
            Despawn();
        }
    }

    public void NewCharge(int oldCharge, int newCharge)
    {
        bool newDark = newCharge == 0;
        if (newDark == dark) return;
        Locomotion l = GetComponent<Locomotion>();
        if (newDark)
        {
            l.BaseSpeed = darkSpeed;
            attackDelay = darkAttackDelay;
            transform.localScale *= darkSize;
            renderer.sprite = darkSprite;
            eyesRenderer.sprite = darkEyesSprite;
            attackKnockback = darkAttackKnockback;
            GetComponent<Rigidbody2D>().mass = darkMass;
        } else
        {
            l.BaseSpeed = lightSpeed;
            attackDelay = lightAttackDelay;
            transform.localScale /= darkSize;
            renderer.sprite = lightSprite;
            eyesRenderer.sprite = lightEyesSprite;
            attackKnockback = lightAttackKnockback;
            GetComponent<Rigidbody2D>().mass = lightMass;
        }
        dark = newDark;
    }
}
