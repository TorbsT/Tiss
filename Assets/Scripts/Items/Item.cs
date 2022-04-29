using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Item")]
public class Item : ScriptableObject
{
    public GameObject Prefab => prefab;
    public Sprite InventorySprite => inventorySprite;
    public string Id => id;
    public string Tags => tags;
    public string Name { get { if (name == "") return id; return name; } }
    public int StackSize => stackSize;

    [SerializeField] private new string name;
    [SerializeField] private Sprite inventorySprite;
    [SerializeField] private GameObject prefab;
    [SerializeField] private string id;
    [SerializeField] private string tags;
    [SerializeField, Range(1, 1024)] private int stackSize = 1;
}
