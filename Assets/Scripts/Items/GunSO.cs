using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Gun")]
public class GunSO : ScriptableObject
{
    public float ShotDelay => shotDelay;
    public float ShotSpread => shotSpread;
    public int ShotCount => shotCount;
    public bool Auto => auto;

    [SerializeField] private float shotDelay;
    [SerializeField] private float shotSpread;  // In degrees (celsius)
    [SerializeField] private int shotCount;
    [SerializeField] private bool auto;
}
