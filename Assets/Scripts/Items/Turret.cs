using EzPools;
using Pools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour, IInteractable, IInventoryListener
{
    public InventoryObject GunInventory => gunInventory;
    public InventoryObject AmmoInventory => ammoInventory;

    public Vector3 Position => transform.position;

    [SerializeField] private bool autofire;
    [SerializeField] private InventoryObject gunInventory;
    [SerializeField] private InventoryObject ammoInventory;
    [SerializeField] private Transform child;
    private Tooltip tooltip;
    private ICollection<Gun> childGuns = new HashSet<Gun>();

    void Awake()
    {
        gunInventory = ScriptableObject.CreateInstance<InventoryObject>();
        gunInventory.SlotCount = 1;
    }
    void Start()
    {
        gunInventory.AddListener(this);
    }
    private void Update()
    {
        if (autofire) TryShoot();
    }
    void OnTransformChildrenChanged()
    {
        AttachedChanged();
    }
    private void AttachedChanged()
    {
        childGuns = transform.GetComponentsInChildren<Gun>();

        foreach (Drop drop in GetComponentsInChildren<Drop>())
        {
            drop.Active = false;
            drop.transform.localPosition = Vector2.zero;
            drop.transform.localRotation = Quaternion.identity;
        }
    }
    private void TryShoot()
    {
        foreach (Gun gun in childGuns)
        {
            if (gun.CanShoot) gun.Shoot();
        }
    }

    public bool CanHover(Interactor interactor)
    {
        return true;
    }

    public bool CanInteract(Interactor interactor)
    {
        return true;
    }

    public void Hover(Interactor interactor)
    {
        tooltip = TooltipPool.Instance.Depool();
        tooltip.KeyCode = KeyCode.F;
        tooltip.transform.SetParent(UI.Instance.transform, false);
        tooltip.PositionToDisplay = new Vector2(transform.position.x, transform.position.y+1f);
    }

    public void Unhover(Interactor interactor)
    {
        TooltipPool.Instance.Enpool(tooltip);
    }

    public void Interact(Interactor interactor)
    {
        Hotbar hb = interactor.GetComponent<Hotbar>();
        int index = hb.ChosenIndex;

        InventoryExtensions.Swap(gunInventory, 0, interactor.GetComponent<PlayerInventoryAPI>().Main, index);
    }
    public void StateChanged(InventoryObject inv)
    {
        if (child != null)
            Manager.Instance.Enpool(child.gameObject);
        Item item = gunInventory.GetSlotItem(0);
        if (item == null)
        {
            child = null;
        }
        else
        {
            GameObject go = Manager.Instance.Depool(item.Prefab);
            child = go.transform;
            child.SetParent(transform);
        }
    }
}
