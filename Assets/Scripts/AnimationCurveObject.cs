using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="SO/AnimationCurve")]
public class AnimationCurveObject : ScriptableObject
{
    public AnimationCurve Curve => curve;
    [SerializeField] private AnimationCurve curve;
}
