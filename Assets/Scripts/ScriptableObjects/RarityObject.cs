using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Scriptable Object/Rarity")]
    public class RarityObject : ScriptableObject
    {
        [field: SerializeField] public Color Color { get; private set; } = Color.red;
    }
}
