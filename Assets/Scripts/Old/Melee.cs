using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    [SerializeField] private bool gizmos;
    [SerializeField, Range(0f, 10f)] private float collisionDuration = 0.2f;
    [SerializeField] private bool useRaycast;
    [SerializeField] private Animator animator;
    [SerializeField, Range(0f, 100f)] private float range = 100f;
    [SerializeField, Range(0f, 1000f)] private float knockback = 100f;

    private Vector2 origin;
    private Vector2 aim;
    private Vector2 hitPos;
    [SerializeField] private float detectCollisionTime;
    private new Collider2D collider;

    private void Awake()
    {
        collider = GetComponent<Collider2D>();
    }
    private void OnDrawGizmos()
    {
        if (gizmos)
        {
            Gizmos.color = Color.blue;

            Gizmos.DrawLine(origin, (aim-origin).normalized*range+origin);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(origin, hitPos);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (detectCollisionTime <= 0f) return;
        Collider2D collider = collision.collider;
        if (collider != null)
        {
            if (collider.GetComponent<Zombie>() == null) return;
            //detectCollisionTime = 0f;
            //this.collider.enabled = false;
            hitPos = collider.transform.position;
            collider.attachedRigidbody.AddForce((hitPos - origin) * knockback);
            collider.GetComponent<HP>()?.Decrease(20);
            ParticleSystem.Particle = "blood";
            ParticleSystem.Pos = hitPos;
            ParticleSystem.Spawn();
        }
    }
    private void Update()
    {
        detectCollisionTime -= Time.deltaTime;
        collider.enabled = detectCollisionTime > 0f;
    }
    public void Shoot()
    {
        if (useRaycast) RaycastShoot();
        else CollisionShoot();
    }

    private void CollisionShoot()
    {
        animator.SetTrigger("Shoot");
        origin = Player.Instance.transform.position;
        detectCollisionTime = collisionDuration;
    }
    private void RaycastShoot()
    {
        animator.SetTrigger("Shoot");
        origin = Player.Instance.transform.position;
        aim = UI.Instance.RectCalculator.ScreenPointToWorld(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(origin, aim - origin, range, 1 << LayerManager.ZombieLayer);
        if (hit.collider != null)
        {
            hitPos = hit.transform.position;
            hit.collider.attachedRigidbody.AddForce((hitPos - origin) * knockback);
            hit.collider.GetComponent<HP>()?.Decrease(20);
        }
    }
}
