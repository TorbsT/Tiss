using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    public static int DroppedItemLayer => Instance.droppedItemLayer;
    public static int ZombieLayer => Instance.zombieLayer;
    private static LayerManager Instance { get; set; }

    [SerializeField] private int droppedItemLayer;
    [SerializeField] private int zombieLayer;

    private void Awake()
    {
        Instance = this;
    }
}
