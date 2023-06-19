using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Scriptable Object/Targeter")]
    public class TargeterObject : ScriptableObject
    {
        /// <summary>
        /// MaxTargets = -1 means unlimited targets
        /// </summary>
        [field: Range(-1, 10)] public int MaxTargets { get; set; } = 1;
        [field: Range(-1, 10)] public float Range { get; set; } = 2;
    }
}
