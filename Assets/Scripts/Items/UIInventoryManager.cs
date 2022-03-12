using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryManager : MonoBehaviour, IHotbarListener
{
    // Display all relevant UIInventories, handle moving items, handle equipping items
    // Start is called before the first frame update
    public static UIInventoryManager Instance { get; private set; }

    private PlayerInventoryAPI api;
    [SerializeField] private UIInventory mainUIInv;
    [SerializeField] private UIInventory movingUIInv;
    [SerializeField] private Transform hotbarChooserTransform; 

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        SetAPI(PlayerInventoryAPI.Instance);
    }

    public void SetAPI(PlayerInventoryAPI api)
    {
        mainUIInv.SetInventory(api.Main);
        movingUIInv.SetInventory(api.Moving);
        api.Hotbar.AddListener(this);
        this.api = api;
    }
    // Update is called once per frame
    void Update()
    {
        movingUIInv.transform.position = Input.mousePosition;

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            DropMoving();
        }
    }

    private void DropMoving()
    {
        Item itm = movingUIInv.Inventory.GetSlotItem(0);
        int quantity = movingUIInv.Inventory.GetSlotQuantity(0);

        InventoryExtensions.QuickMove(movingUIInv.Inventory, 0, api.Dropper);
    }
    public void Clicked(UIItemSlot slot, UIInventory inventory)
    {
        InventoryObject moving = movingUIInv.Inventory;
        InventoryObject subject = inventory.Inventory;

        int movingIndex = 0;
        int subjectIndex = slot.Index;
        Item movingItem = moving.GetSlotItem(movingIndex);
        Item subjectItem = subject.GetSlotItem(subjectIndex);

        if (movingItem == subjectItem && movingItem != null)
        {
            InventoryExtensions.Merge(moving, movingIndex, subject, subjectIndex);
        } else
        {
            InventoryExtensions.Swap(moving, movingIndex, subject, subjectIndex);
        }
    }

    public void StateChanged(Hotbar hotbar)
    {
        int index = hotbar.ChosenIndex;
        Transform uiSlotTransform = mainUIInv.transform.GetChild(index);
        hotbarChooserTransform.position = uiSlotTransform.position;
    }
}
