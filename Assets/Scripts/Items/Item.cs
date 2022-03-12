using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Item")]
public class Item : ScriptableObject
{
    public GameObject Prefab => prefab;
    public Sprite InventorySprite => inventorySprite;
    public string Id => id;
    public int StackSize => stackSize;

    [SerializeField] private Sprite inventorySprite;
    [SerializeField] private GameObject prefab;
    [SerializeField] private string id;
    [SerializeField, Range(1, 1024)] private int stackSize = 1;
}
