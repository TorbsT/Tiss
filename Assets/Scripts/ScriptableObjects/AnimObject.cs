using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Scriptable Object/Anim")]
    public class AnimObject : ScriptableObject
    {
        [field: SerializeField]
        public AnimationCurve Curve { get; private set; }
        = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [field: SerializeField]
        public float Duration { get; private set; } = 1f;
    }
}
