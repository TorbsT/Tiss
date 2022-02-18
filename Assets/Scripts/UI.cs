using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public RectCalculator RectCalculator => rectCalculator;

    public static UI Instance { get; private set; }
    private RectCalculator rectCalculator;
    private void Awake()
    {
        Instance = this;
        rectCalculator = GetComponent<RectCalculator>();
    }
}
