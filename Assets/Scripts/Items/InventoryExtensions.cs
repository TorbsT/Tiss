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
    public static void Flush(InventoryObject inventory)
    {
        for (int i = 0; i < inventory.SlotCount; i++)
        {
            inventory.Set(i, null, 0);
        }
    }

    public static void QuickRemove(InventoryObject inventory, Item item, int count)
    {
        int left = count;
        for (int i = inventory.SlotCount-1; i >= 0; i--)
        {
            if (inventory.GetSlotItem(i) == item)
            {
                int current = inventory.GetSlotQuantity(i);
                if (current > 0)
                {
                    int used = Mathf.Min(current, left);
                    inventory.Set(i, item, current - used);
                    left -= used;
                    if (left == 0) break;
                }
            }
        }
    }
    public static void QuickRemove(InventoryObject inventory, int index, int count)
    {
        QuickRemove(inventory, inventory.GetSlotItem(index), count);
    }

    public static bool CanQuickRemove(InventoryObject inventory, Item item, int count)
    {
        int left = count;
        if (left == 0) return true;
        for (int i = inventory.SlotCount - 1; i >= 0; i--)
        {
            if (inventory.GetSlotItem(i) == item)
            {
                int current = inventory.GetSlotQuantity(i);
                if (current > 0)
                {
                    int used = Mathf.Min(current, left);
                    left -= used;
                    if (left == 0) return true;
                }
            }
        }
        return false;
    }
    /**
     * Merges item a into item b if they are the same.
     * If they aren't the same, swap
     */
    public static void MergeElseSwap(InventoryObject oa, int ia, InventoryObject ob, int ib)
    {
        if (SameItem(oa, ia, ob, ib))
        {
            // Merge
            Merge(oa, ia, ob, ib);
        } else
        {
            // Swap
            Swap(oa, ia, ob, ib);
        }
    }
    public static bool SameItem(InventoryObject oa, int ia, InventoryObject ob, int ib)
    {
        if (oa == null || ob == null) Debug.LogError("From-inventory is " + oa + ", To-inventory is " + ob);
        return oa.GetSlotItem(ia) == ob.GetSlotItem(ib);
    }
}
