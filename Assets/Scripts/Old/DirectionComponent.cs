using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionComponent : MonoBehaviour
{
    public Room.Direction RelativeDirection => relativeDirection;
    [SerializeField] private Room.Direction relativeDirection;
}
