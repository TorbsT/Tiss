using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotbar : MonoBehaviour, IInventoryListener
{
    public int ChosenIndex => chosenIndex;
    public Item ChosenItem => chosenItem;
    [SerializeField] private Item chosenItem;
    [SerializeField] private int chosenIndex;
    private int slotCount;
    private InventoryObject inventory;
    private HashSet<IHotbarListener> listeners = new();


    // Start is called before the first frame update
    void Start()
    {
        inventory = GetComponent<PlayerInventoryAPI>().Main;
        inventory.AddListener(this);
    }

    // Update is called once per frame
    void Update()
    {
        int newChosenIndex = chosenIndex;
        if (Input.GetKeyDown(KeyCode.Alpha1)) newChosenIndex = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2)) newChosenIndex = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3)) newChosenIndex = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha4)) newChosenIndex = 3;
        else if (Input.GetKeyDown(KeyCode.Alpha5)) newChosenIndex = 4;
        else if (Input.GetKeyDown(KeyCode.Alpha6)) newChosenIndex = 5;
        else if (Input.GetKeyDown(KeyCode.Alpha7)) newChosenIndex = 6;
        else if (Input.GetKeyDown(KeyCode.Alpha8)) newChosenIndex = 7;
        else if (Input.GetKeyDown(KeyCode.Alpha9)) newChosenIndex = 8;

        if (newChosenIndex != chosenIndex)
        {
            ChooseIndex(newChosenIndex);
        }
    }

    public void AddListener(IHotbarListener listener)
    {
        listeners.Add(listener);
        FireStateChanged();
    }
    public void RemoveListener(IHotbarListener listener)
    {
        listeners.Remove(listener);
    }
    public void StateChanged(InventoryObject obj)
    {
        this.inventory = obj;
        slotCount = obj.SlotCount;
        ChooseIndex(Mathf.Min(slotCount - 1, chosenIndex));
    }
    private void ChooseIndex(int index)
    {
        int oldChosenIndex = chosenIndex;
        chosenIndex = index;
        chosenItem = inventory.GetSlotItem(index);
        FireStateChanged();
    }
    private void FireStateChanged()
    {
        foreach (IHotbarListener listener in listeners)
        {
            listener.StateChanged(this);
        }
    }
}
