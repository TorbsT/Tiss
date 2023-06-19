using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class Room : MonoBehaviour
    {
        [field: SerializeField] public SpriteRenderer FogRenderer { get; private set; }
        [field: SerializeField] public GameObject TowerGreen { get; private set; }
        [field: SerializeField] public GameObject TowerRed { get; private set; }
        [field: SerializeField] public float Disturbance { get; set; }
        [field: SerializeField] public int Rotation { get; set; }
        [field: SerializeField] public Vector2Int Loc { get; set; }
    }
}
