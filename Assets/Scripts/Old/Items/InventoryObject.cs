using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="SO/Inventory")]
public class InventoryObject : ScriptableObject
{
    public int SlotCount { get => slotCount; set { slotCount = value; } }

    [SerializeField] private int slotCount;
    [SerializeField] private List<Slot> slots = new();
    private HashSet<IInventoryListener> listeners = new();

    public void AddListener(IInventoryListener listener)
    {
        listeners.Add(listener);
        FireStateChanged();
    }
    public void RemoveListener(IInventoryListener listener)
    {
        listeners.Remove(listener);
    }

    // No quickadd or quickremove here - do that for managers
    public void Set(int index, Item item, int quantity)
    {
        UpdateList();

        GetSlot(index).Set(item, quantity);

        FireStateChanged();
    }

    public int GetItemQuantity(Item item)
    {
        int quantity = 0;
        foreach (Slot slot in slots)
        {
            if (slot.Item == item) quantity += slot.Quantity;
        }
        return quantity;
    }
    public int GetSlotQuantity(int index)
    {
        return GetSlot(index).Quantity;
    }
    public Item GetSlotItem(int index)
    {
        return GetSlot(index).Item;
    }


    private Slot GetSlot(int index)
    {
        UpdateList();
        EnsureIndex(index);
        return slots[index];
    }
    private void EnsureIndex(int index)
    {
        if (slotCount != slots.Count) Debug.LogError("What is this sorcery " + index + ", " + slotCount + " " + slots.Count);
        if (index < 0 || index >= slotCount) Debug.LogError("Invalid index " + index + " - slotCount is " + slotCount);
    }

    /**
     * Update list to match slotCount in inspector
    */
    private void UpdateList()
    {
        if (slotCount != slots.Count)
        {
            if (slotCount > slots.Count)
            {
                while (slots.Count < slotCount)
                {
                    slots.Add(new());
                }
            }
            if (slotCount < slots.Count)
            {
                slots.RemoveRange(slotCount, slots.Count - slotCount);
            }
        }
    }
    private void FireStateChanged()
    {
        foreach (IInventoryListener listener in listeners)
        {
            listener.StateChanged(this);
        }
    }

    [System.Serializable]
    private class Slot
    {
        public enum State
        {
            empty,
            taken,
            full
        }

        public Item Item => item;
        public int Quantity => quantity;
        public State CurrentState => state;

        [SerializeField] private Item item;
        [SerializeField] private int quantity;
        [SerializeField] private State state;

        public void Set(Item item, int quantity)
        {
            if (item == null || quantity <= 0)
            {
                quantity = 0;
                item = null;
            }
            if (quantity == 0)
            {
                state = State.empty;
            }
            else if (quantity >= item.StackSize)
            {
                state = State.full;
                quantity = item.StackSize;
            }
            else
            {
                state = State.taken;
            }

            this.item = item;
            this.quantity = quantity;
        }
    }
}
