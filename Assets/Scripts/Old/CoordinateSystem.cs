using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinateSystem : MonoBehaviour
{
    private static CoordinateSystem Instance { get; set; }
    private ICoordinateSystem system;
    private void Awake()
    {
        Instance = this;
        system = GetComponent<ICoordinateSystem>();
    }
    public static Vector2 LocToPos(Vector2Int loc) => Instance.system.LocToPos(loc);
}
public interface ICoordinateSystem
{
    Vector2 LocToPos(Vector2Int loc);
}
