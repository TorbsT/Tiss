using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Scriptable Object/Locomotion")]
    public class LocomotionObject : ScriptableObject
    {
        [field: SerializeField, Range(0f, 3f)] public float BaseSpeed { get; set; } = 0.25f;
        [field: SerializeField] public float Acceleration { get; set; } = 100f;
        [field: SerializeField] public float Deceleration { get; set; } = 100f;
    }
}
