using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InventoryExtensions
{
    public static void Swap(InventoryObject oa, int ia, InventoryObject ob, int ib)
    {
        Item itemA = oa.GetSlotItem(ia);
        int quantityA = oa.GetSlotQuantity(ia);
        Item itemB = ob.GetSlotItem(ib);
        int quantityB = ob.GetSlotQuantity(ib);

        oa.Set(ia, itemB, quantityB);
        ob.Set(ib, itemA, quantityA);
    }
    public static void QuickMove(InventoryObject oa, int ia, IItemReceiver receiver)
    {
        Item itemA = oa.GetSlotItem(ia);
        int quantityA = oa.GetSlotQuantity(ia);

        receiver.Add(itemA, quantityA);
        oa.Set(ia, null, 0);
    }
    public static void Merge(InventoryObject oa, int ia, InventoryObject ob, int ib)
    {
        Item itemA = oa.GetSlotItem(ia);
        int quantityA = oa.GetSlotQuantity(ia);
        Item itemB = ob.GetSlotItem(ib);
        int quantityB = ob.GetSlotQuantity(ib);
        if (itemA == null && itemB == null) return;
        
        int free = itemA.StackSize-quantityB;

        int used = Mathf.Min(free, quantityA);
        oa.Set(ia, itemA, quantityA-used);
        ob.Set(ib, itemB, quantityB+used);
    }
    public static int QuickAdd(InventoryObject inventory, Item item, int quantity)
    {
        if (inventory == null) Debug.LogError("Can't quickadd to null inventory");

        // Add to preexisting stacks
        for (int i = 0; i < inventory.SlotCount; i++)
        {
            if (quantity <= 0) break;
            Item itm = inventory.GetSlotItem(i);
            if (itm == item)
            {
                int current = inventory.GetSlotQuantity(i);
                int free = item.StackSize-current;
                if (free > 0)
                {
                    int used = Mathf.Min(free, quantity);
                    inventory.Set(i, item, current + used);
                    quantity -= used;
                }
            }
        }

        // Add to new stacks
        for (int i = 0; i < inventory.SlotCount; i++)
        {
            if (quantity <= 0) break;
            Item itm = inventory.GetSlotItem(i);
            if (itm == null)
            {
                int free = item.StackSize;
                if (free > 0)
                {
                    int used = Mathf.Min(free, quantity);
                    inventory.Set(i, item, used);
                    quantity -= used;
                }
            }
        }


        // Returns remaining quantity that couldn't be added
        return quantity;
    }
}