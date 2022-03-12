using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Flash")]
public class FlashObject : ScriptableObject
{
    public float FlashDuration => flashDuration;
    public Gradient ColorGradient => colorGradient;
    public AnimationCurve IntensityCurve => intensityCurve;
    public float MaxIntensity => maxIntensity;

    [SerializeField] private float flashDuration;
    [SerializeField] private Gradient colorGradient;
    [SerializeField] private AnimationCurve intensityCurve;
    [SerializeField] private float maxIntensity;
}
