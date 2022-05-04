using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour, IShopListener, IButtonReceiver
{
    public static ShopUI Instance { get; private set; }

    [Header("CONFIG")]
    [SerializeField] private int alwaysUnlockedSlots;

    [Header("Panels")]
    [SerializeField] private RectTransform welcomePanel;
    [SerializeField] private RectTransform inspectPanel;
    [SerializeField] private RectTransform inventoryPanel;
    [SerializeField] private RectTransform inspectBorder;

    [Header("Inspect Panel")]
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI buttonField;
    [SerializeField] private TextMeshProUGUI titleField;
    [SerializeField] private TextMeshProUGUI descField;
    [SerializeField] private UIItemSlot inspectSlot;

    [Header("Debug")]
    [SerializeField] private int chosenIndex = -1;
    [SerializeField] private Item chosenItem;
    [SerializeField] private int chosenQuantity;
    [SerializeField] private int chosenCost;
    [SerializeField] private string chosenTitle;
    [SerializeField] private string chosenDesc;

    private Shop shop;
    private List<UIItemSlot> inventorySlots;
    private UIItemSlot lastChosenSlot;

    private void Awake()
    {
        Instance = this;
        inventorySlots = new();
        foreach (UIItemSlot slot in inventoryPanel.GetComponentsInChildren<UIItemSlot>())
        {
            inventorySlots.Add(slot);
        }
        Close();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Click(UIItemSlot slot, UIInventory inv)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (slot == inventorySlots[i])
            {
                Choose(i);
                return;
            } 
        }
    }
    public void Choose(int index)
    {
        chosenIndex = index;
        Refresh();
    }
    public void Buy()
    {
        if (shop == null)
        {
            Debug.LogWarning("ShopUI has no Shop");
            return;
        }
        PlayerInventoryAPI.Instance.Wallet.Shitcoin -= chosenCost;
        InventoryExtensions.QuickAdd(PlayerInventoryAPI.Instance.Main, chosenItem, chosenQuantity);
    }
    public void Open(Shop shop)
    {
        Close();
        shop.AddListener(this);
        gameObject.SetActive(true);
    }
    public void Close()
    {
        chosenIndex = -1;
        chosenItem = null;
        chosenCost = -1;
        chosenQuantity = -1;
        if (this.shop != null)
        {
            shop.RemoveListener(this);
            this.shop = null;
        }
        gameObject.SetActive(false);
    }
    public void StateChanged(Shop shop)
    {
        this.shop = shop;
        Refresh();
    }
    private void Refresh()
    {
        if (shop == null)
        {
            Debug.LogWarning("ShopUI has no Shop");
            return;
        }
        if (lastChosenSlot != null)
        {
            lastChosenSlot.Unmark();
        }

        chosenItem = shop.GetItem(chosenIndex);
        chosenCost = shop.GetCost(chosenIndex);
        chosenQuantity = shop.GetQuantity(chosenIndex);
        chosenTitle = shop.GetTitle(chosenIndex);
        chosenDesc = shop.GetDesc(chosenIndex);

        bool powered = shop.Powered;
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            Item item = shop.GetItem(i);
            int quantity = shop.GetQuantity(i);
            UIItemSlot slot = inventorySlots[i];
            slot.SetItem(item);
            slot.SetQuantity(quantity);
            bool locked = !powered && i >= alwaysUnlockedSlots;
            slot.SetLocked(locked);
        }


        bool chosen = chosenItem != null;
        welcomePanel.gameObject.SetActive(!chosen);
        inspectPanel.gameObject.SetActive(chosen);
        inspectBorder.gameObject.SetActive(chosen);

        if (chosen)
        {
            bool locked = inventorySlots[chosenIndex].Locked;
            buttonField.text = "$" + chosenCost;
            button.interactable = !locked;
            titleField.text = chosenTitle;
            if (locked) descField.text = "This item is only available when the shop is powered by a nearby generator.";
            else descField.text = chosenDesc;
            inspectSlot.SetItem(chosenItem);
            inspectSlot.SetQuantity(chosenQuantity);
            lastChosenSlot = inventorySlots[chosenIndex];
            lastChosenSlot.Mark();
        } else
        {
            lastChosenSlot = null;
        }
    }
}
