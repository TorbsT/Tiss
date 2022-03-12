using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pools;

public class Bullet : MonoBehaviour
{
    public BulletSO SO { get => so; set { so = value; } }
    [SerializeField] private BulletSO so;
    [SerializeField] private float life;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Transform t = collision.collider.transform;
        Rigidbody2D rb = t.GetComponent<Rigidbody2D>();
        if (rb != null) t.position += transform.right * so.Knockback;
        Zombie z = t.GetComponent<Zombie>();
        if (z != null) z.Damage(so.Damage);
        Despawn();
    }
    private void OnEnable()
    {
        life = 0f;
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
        if (so == null) return;
        transform.position += transform.right * so.Speed * Time.fixedDeltaTime;
    }
    public void Despawn()
    {
        BulletPool.Instance.Enpool(this);
    }
}
