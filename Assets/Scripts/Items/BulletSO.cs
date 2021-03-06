using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Bullet")]
public class BulletSO : ScriptableObject
{
    public AnimationCurve SpeedCurve => speedCurve;
    public float Speed => speed;
    public float Knockback => knockback;
    public float DespawnTime => despawnTime;
    public float Damage => damage;

    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private float speed;
    [SerializeField] private float knockback;
    [SerializeField] private float despawnTime;
    [SerializeField] private float damage;
}
