using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzPools;

public class UIInventory : MonoBehaviour, IInventoryListener
{
    public static UIInventory Instance { get; private set; }
    public InventoryObject Inventory => inventory;
    private InventoryObject inventory;

    void Awake()
    {
        Instance = this;
    }
    public void SetInventory(InventoryObject newInventory)
    {
        if (inventory != null)
        {
            inventory.RemoveListener(this);
        }
        if (newInventory != null)
        {
            newInventory.AddListener(this);
        }
        inventory = newInventory;
        Rerender();
    }
    public void StateChanged(InventoryObject obj)
    {
        Rerender();
    }
    private void Rerender()
    {
        if (inventory == null)
        {
            Debug.LogWarning(this + " does not have an inventory to render");
            return;
        }
        foreach (UIItemSlot slot in GetComponentsInChildren<UIItemSlot>())
        {
            UIItem old = slot.UIItem;
            if (old != null) Manager.Instance.Enpool(old.gameObject);

            int index = slot.Index;
            UIItem uiitem = Manager.Instance.Depool(Manager.Instance.UIItemPrefab).GetComponent<UIItem>();
            uiitem.Item = inventory.GetSlotItem(index);
            uiitem.Quantity = inventory.GetSlotQuantity(index);
            slot.SetUIItem(uiitem);
        }
    }

}
