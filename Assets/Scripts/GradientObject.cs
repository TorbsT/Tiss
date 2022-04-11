using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/GradientObject")]
public class GradientObject : ScriptableObject
{
    public Gradient Gradient => gradient;
    [SerializeField] private Gradient gradient;
}
