using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Border : MonoBehaviour
{
    public Room.Direction RelativeDirection => directionComponent.RelativeDirection;
    [SerializeField] private DirectionComponent directionComponent;

    private void OnEnable()
    {
        BorderSystem.Instance.Add(this);
    }
    private void OnDisable()
    {
        BorderSystem.Instance.Remove(this);
    }
}
