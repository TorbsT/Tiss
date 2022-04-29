using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurretUI : MonoBehaviour, ITurretListener
{
    public static TurretUI Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI poweredField;
    [SerializeField] private TextMeshProUGUI needsPowerField;
    [SerializeField] private TextMeshProUGUI mountingErrorField;
    [SerializeField] private TextMeshProUGUI mountingSuccessField;
    [SerializeField] private RectTransform gunSlotsWrapper;
    [SerializeField] private RectTransform ammoSlotsWrapper;

    private Turret turret;
    private Interactor interactor;
    private List<UIItemSlot> ammoSlots = new();
    private List<UIItemSlot> gunSlots = new();

    private void Awake()
    {
        Instance = this;
        foreach (UIItemSlot slot in ammoSlotsWrapper.GetComponentsInChildren<UIItemSlot>())
        {
            ammoSlots.Add(slot);
        }
        foreach (UIItemSlot slot in gunSlotsWrapper.GetComponentsInChildren<UIItemSlot>())
        {
            gunSlots.Add(slot);
        }
        gameObject.SetActive(false);
    }

    public void Open(Turret turret, Interactor interactor)
    {
        bool justClosing = this.turret != null;
        Close();
        if (!justClosing)
        {
            this.turret = turret;
            this.interactor = interactor;
            gameObject.SetActive(true);
            turret.AddListener(this);
        }
    }
    public void Close()
    {
        if (this.turret != null) this.turret.RemoveListener(this);
        this.turret = null;
        this.interactor = null;
        gameObject.SetActive(false);
    }

    public void ClickGunSlot(UIItemSlot slot)
    {
        InventoryObject moving = interactor.GetComponent<PlayerInventoryAPI>().Moving;
        Item inputItem = moving.GetSlotItem(0);
        if (inputItem == null || inputItem.Tags.Contains("gun"))
        {
            InventoryExtensions.MergeElseSwap(moving, 0, turret.GunInventory, slot.Index);
        }
    }
    public void ClickAmmoSlot(UIItemSlot slot)
    {
        InventoryObject moving = interactor.GetComponent<PlayerInventoryAPI>().Moving;
        Item inputItem = moving.GetSlotItem(0);
        if (inputItem == null || inputItem.Tags.Contains("ammo"))
        {
            InventoryExtensions.MergeElseSwap(moving, 0, turret.AmmoInventory, slot.Index);
        }
    }

    public void StateChanged(Turret turret)
    {
        Item gunItem = turret.GunInventory.GetSlotItem(0);

        bool mountingProblem = true;
        bool batteryProblem = !turret.Powered;
        if (gunItem == null)
        {
            mountingErrorField.text = "Requires a gun to function";
        } else
        {
            GunSO gunSO = gunItem.Prefab.GetComponent<Gun>().GunSO;
            Item requiredAmmoItem = gunSO.Ammo;

            int requiredAmount = Mathf.CeilToInt(gunSO.AmmoPerBurst);
            if (InventoryExtensions.CanQuickRemove(turret.AmmoInventory, requiredAmmoItem, requiredAmount))
            {
                // can shoot
                mountingProblem = false;
            } else
            {
                // Invalid ammo
                mountingErrorField.text = gunItem.Name + " needs more " + requiredAmmoItem.Name + " to shoot";
            }
        }

        mountingErrorField.gameObject.SetActive(mountingProblem);
        mountingSuccessField.gameObject.SetActive(!mountingProblem);
        poweredField.gameObject.SetActive(!batteryProblem);
        needsPowerField.gameObject.SetActive(batteryProblem);

        for (int i = 0; i < gunSlots.Count; i++)
        {
            UIItemSlot slot = gunSlots[i];
            InventoryObject io = turret.GunInventory;
            slot.SetItem(io.GetSlotItem(i));
            slot.SetQuantity(io.GetSlotQuantity(i));
        }
        for (int i = 0; i < ammoSlots.Count; i++)
        {
            UIItemSlot slot = ammoSlots[i];
            InventoryObject io = turret.AmmoInventory;
            slot.SetItem(io.GetSlotItem(i));
            slot.SetQuantity(io.GetSlotQuantity(i));
        }
    }
}
