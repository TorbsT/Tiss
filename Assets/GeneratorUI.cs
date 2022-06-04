using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GeneratorUI : MonoBehaviour, IGeneratorListener
{
    public static GeneratorUI Instance { get; private set; }

    [SerializeField] private float range;
    [SerializeField] private Gradient batteryColors;
    [SerializeField] private GameObject poweredField;
    [SerializeField] private GameObject chargingField;
    [SerializeField] private GameObject needsGasField;
    [SerializeField] private TextMeshProUGUI batteryField;
    [SerializeField] private Image batteryImg;
    [SerializeField] private RectTransform inventoryWrapper;

    [SerializeField] private Generator generator;
    [SerializeField] private Interactor interactor;
    private List<UIItemSlot> slots;

    private void Awake()
    {
        Instance = this;
        slots = new();
        foreach (UIItemSlot slot in inventoryWrapper.GetComponentsInChildren<UIItemSlot>())
        {
            slots.Add(slot);
        }
        gameObject.SetActive(false);
    }
    void OnEnable()
    {
        Generator generator = PopupSystem.Instance.CurrentRequest.interacted.GetComponent<Generator>();
        Interactor interactor = PopupSystem.Interactor;
        bool onlyClose = this.generator != null;
        Close();
        if (!onlyClose)
        {
            this.generator = generator;
            this.interactor = interactor;
            generator.AddListener(this);
        }
    }
    void OnDisable()
    {
        Close();
    }
    private void Close()
    {
        if (generator != null)
        generator.RemoveListener(this);
        generator = null;
        interactor = null;
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
