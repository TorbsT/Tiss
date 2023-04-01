using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    public static int DroppedItemLayer => Instance.droppedItemLayer;
    public static int WallLayer => Instance.wallLayer;
    public static int ZombieLayer => Instance.zombieLayer;
    public static int BarricadeLayer => Instance.barricadeLayer;
    private static LayerManager Instance { get; set; }

    [SerializeField] private int droppedItemLayer;
    [SerializeField] private int wallLayer;
    [SerializeField] private int zombieLayer;
    [SerializeField] private int barricadeLayer;

    private void Awake()
    {
        Instance = this;
    }
}
