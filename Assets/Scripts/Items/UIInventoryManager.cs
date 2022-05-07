using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryManager : MonoBehaviour, IHotbarListener, IButtonReceiver
{
    // Display all relevant UIInventories, handle moving items, handle equipping items
    // Start is called before the first frame update
    public static UIInventoryManager Instance { get; private set; }

    private PlayerInventoryAPI api;
    [SerializeField] private UIInventory mainUIInv;
    [SerializeField] private UIInventory movingUIInv;
    [SerializeField] private Transform hotbarChooserTransform;
    [SerializeField] private Vector2 chooserOffset;

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

        if (false && Input.GetKeyDown(KeyCode.Mouse1))
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
    public void Click(UIItemSlot slot, UIInventory inventory)
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
        if (mainUIInv != null)
        {
            Transform uiSlotTransform = mainUIInv.transform.GetChild(index);
            Vector2 slotPos = uiSlotTransform.position;
            hotbarChooserTransform.position = slotPos+chooserOffset;
        }

    }
}
