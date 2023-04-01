using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBullet : MonoBehaviour
{
    [SerializeField] private Gradient startColors;
    [SerializeField] private AnimationCurve smokeCurve;

    private Bullet bullet;
    private Color startColor;
    private float maxLife;
    private new SpriteRenderer renderer;
    private Rigidbody2D rb;

    private void Awake()
    {
        bullet = GetComponent<Bullet>();
        startColor = startColors.Evaluate(Random.Range(0f, 1f));
        renderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float c = smokeCurve.Evaluate(bullet.Life / bullet.SO.DespawnTime);
        renderer.color = new Color(startColor.r * c, startColor.g*c, startColor.b*c);
        Vector2 added = new Vector2(Random.value - 0.5f, Random.value - 0.5f) * 0.1f;
        rb.velocity += added;
    }
}
