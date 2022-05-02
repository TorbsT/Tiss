using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GeneratorUI : MonoBehaviour, IGeneratorListener
{
    public static GeneratorUI Instance { get; private set; }

    [SerializeField] private Gradient batteryColors;
    [SerializeField] private GameObject poweredField;
    [SerializeField] private GameObject chargingField;
    [SerializeField] private GameObject needsGasField;
    [SerializeField] private TextMeshProUGUI batteryField;
    [SerializeField] private Image batteryImg;
    [SerializeField] private RectTransform inventoryWrapper;

    private Generator generator;
    private Interactor interactor;
    private List<UIItemSlot> slots;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
        slots = new();
        foreach (UIItemSlot slot in inventoryWrapper.GetComponentsInChildren<UIItemSlot>())
        {
            slots.Add(slot);
        }
    }

    public void Open(Generator generator, Interactor interactor)
    {
        bool onlyClose = this.generator != null;
        Close();
        if (!onlyClose)
        {
            this.generator = generator;
            this.interactor = interactor;
            gameObject.SetActive(true);
            generator.AddListener(this);
        }
    }
    public void Close()
    {
        if (generator != null)
        generator.RemoveListener(this);
        generator = null;
        interactor = null;
        gameObject.SetActive(false);
    }
    public void Clicked(UIItemSlot slot)
    {
        InventoryObject moving = interactor.GetComponent<PlayerInventoryAPI>().Moving;
        Item inputItem = moving.GetSlotItem(0);
        if (inputItem == generator.FuelItem)
        {
            InventoryExtensions.MergeElseSwap(moving, 0, generator.Inventory, slot.Index);
        }
    }
    void IGeneratorListener.StateChanged(Generator generator)
    {
        float charge = this.generator.NewFuel;
        int visualCharge = Mathf.CeilToInt(charge);
        bool powered = visualCharge > 0;
        bool charging = InventoryExtensions.CanQuickRemove(this.generator.Inventory, this.generator.FuelItem, 1);
        poweredField.SetActive(powered && !charging);
        chargingField.SetActive(powered && charging);
        needsGasField.SetActive(!powered);

        batteryField.text = visualCharge + "%";
        batteryImg.fillAmount = charge / 100f;
        batteryImg.color = batteryColors.Evaluate(charge / 100f);

        for (int i = 0; i < slots.Count; i++)
        {
            UIItemSlot slot = slots[i];
            slot.SetItem(this.generator.Inventory.GetSlotItem(i));
            slot.SetQuantity(this.generator.Inventory.GetSlotQuantity(i));
        }
    }
}
