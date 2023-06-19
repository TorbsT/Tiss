using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Scriptable Object/Look")]
    public class LookObject : ScriptableObject
    {
        [field: SerializeField] public float TurnSpeed { get; set; } = 10f;
    }
}
