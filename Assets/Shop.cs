using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour, IBatteryListener
{
    [System.Serializable]
    private class Offer
    {
        [SerializeField] public string title;
        [SerializeField] public string desc;
        [SerializeField] public Item item;
        [SerializeField] public int cost;
        [SerializeField] public int quantity;
    }

    public bool Powered => powered;

    private HashSet<IShopListener> listeners = new();
    private bool powered;
    [SerializeField] private float loseUIRange;
    [SerializeField] private int availableCount = 1;
    [SerializeField] private ShopPoolObject itemPool;
    [SerializeField] private List<Offer> offers;
    [SerializeField] private Transform interactorTransform;

    private void Start()
    {
        Fullrestock();
    }
    public void Fullrestock()
    {
        offers = new();
        foreach (ShopPoolObject.Spawn spawn in itemPool.Generate(availableCount))
        {
            Offer offer = new();
            offer.item = spawn.Item;
            offer.cost = spawn.Cost;
            offer.quantity = spawn.Quantity;
            offer.title = spawn.Title;
            offer.desc = spawn.Desc;
            offers.Add(offer);
        }
        FireStateChanged();
    }
    public void AddListener(IShopListener listener)
    {
        listeners.Add(listener);
        FireStateChanged();
    }
    public void RemoveListener(IShopListener listener)
    {
        listeners.Remove(listener);
    }
    private void FireStateChanged()
    {
        foreach (IShopListener listener in listeners)
        {
            listener.StateChanged(this);
        }
    }

    public Item GetItem(int chosenIndex)
    {
        if (!InRange(chosenIndex)) return null;
        return offers[chosenIndex].item;
    }
    public int GetCost(int chosenIndex)
    {
        if (!InRange(chosenIndex)) return -1;
        return offers[chosenIndex].cost;
    }
    public int GetQuantity(int chosenIndex)
    {
        if (!InRange(chosenIndex)) return -1;
        return offers[chosenIndex].quantity;
    }
    public string GetTitle(int chosenIndex)
    {
        if (!InRange(chosenIndex)) return null;
        return offers[chosenIndex].title;
    }
    public string GetDesc(int chosenIndex)
    {
        if (!InRange(chosenIndex)) return null;
        return offers[chosenIndex].desc;
    }

    private bool InRange(int index)
    {
        return index >= 0 && index < offers.Count;
    }

    public void NewCharge(int oldCharge, int newCharge)
    {
        powered = newCharge > 0;
    }
}
