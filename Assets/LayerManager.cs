using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    public static int DroppedItemLayer => Instance.droppedItemLayer;
    private static LayerManager Instance { get; set; }

    [SerializeField] private int droppedItemLayer;

    private void Awake()
    {
        Instance = this;
    }
}
