using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryAPI : MonoBehaviour, IWalletProvider
{
    public static PlayerInventoryAPI Instance { get; private set; }
    public InventoryObject Main => main;
    public InventoryObject Moving => moving;
    public ItemDropper Dropper => dropper;
    public ItemPlacer Placer => placer;
    public Hotbar Hotbar => hotbar;
    public Wallet Wallet => wallet;

    [SerializeField] private Wallet wallet;
    [SerializeField] private InventoryObject main;
    [SerializeField] private InventoryObject moving;
    private ItemDropper dropper;
    private ItemPlacer placer;
    private Hotbar hotbar;

    private void Awake()
    {
        Instance = this;
        dropper = GetComponent<ItemDropper>();
        hotbar = GetComponent<Hotbar>();
        placer = GetComponent<ItemPlacer>();
    }
    void OnDestroy()
    {
        Instance = null;
    }
}
