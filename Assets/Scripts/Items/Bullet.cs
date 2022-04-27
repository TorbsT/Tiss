using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pools;

public class Bullet : MonoBehaviour
{
    public float Life => life;
    public BulletSO SO { get => so; set { so = value; } }
    private BulletSO so;
    [SerializeField] private float life;
    private bool shotOff = false;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isActiveAndEnabled || !collision.gameObject.activeSelf)
        {
            // I have no idea why this happens but it does sometimes for some reason
            return;
        }
        Transform tr = collision.collider.transform;

        Rigidbody2D rb = tr.GetComponent<Rigidbody2D>();
        if (rb != null) tr.position += transform.right * so.Knockback;
        Team otherTeam = tr.GetComponent<Team>();
        if (otherTeam != null)
        {
            if (GetComponent<Team>().CanInjure(otherTeam))
            {
                otherTeam.GetComponent<HP>().Decrease(so.Damage);
            }
        }
        Despawn();
    }
    private void OnEnable()
    {
        life = 0f;
        shotOff = false;
        
    }
    private void Update()
    {
        if (so == null) return;
        life += Time.deltaTime;
        if (life > so.DespawnTime)
        {
            Despawn();
        }
    }
    private void FixedUpdate()
    {
        if (!shotOff)
        {
            Vector3 v = transform.right * so.Speed;
            rb.velocity = new Vector2(v.x, v.y);
            shotOff = true;
        }
    }
    public void Despawn()
    {
        GetComponent<Destroyable>().Destroy();
    }
}
