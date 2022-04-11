using Pools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Turret : MonoBehaviour, IInventoryListener, IInteractableListener
{
    public InventoryObject GunInventory => gunInventory;
    public InventoryObject AmmoInventory => ammoInventory;

    public Vector3 Position => transform.position;

    [SerializeField] private bool gizmos;
    [SerializeField] private bool autofire;
    [SerializeField] private InventoryObject gunInventory;
    [SerializeField] private InventoryObject ammoInventory;
    [SerializeField] private Transform child;
    [SerializeField] private Transform plate;
    [SerializeField] private Transform target;
    [SerializeField] private float range;
    private Tooltip tooltip;
    private ICollection<Gun> childGuns = new HashSet<Gun>();
    private Collider2D[] targetsInRange;
    private bool[] targetsLOS;

    void Awake()
    {
        gunInventory = ScriptableObject.CreateInstance<InventoryObject>();
        gunInventory.SlotCount = 1;
    }
    void Start()
    {
        gunInventory.AddListener(this);
        GetComponent<Interactable>().AddListener(this);
    }
    void OnDrawGizmosSelected()
    {
        if (!gizmos) return;
        if (targetsInRange != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < targetsInRange.Length; i++)
            {
                Collider2D col = targetsInRange[i];
                bool los = targetsLOS[i];
                Transform t = col.transform;
                if (target == t)
                {
                    Gizmos.color = Color.green;
                } else
                {
                    if (los) Gizmos.color = Color.blue;
                    else Gizmos.color = Color.red;
                }
                Gizmos.DrawLine(transform.position, t.position);
            }
        }
    }
    private void Update()
    {
        int zombieMask = 1 << LayerManager.ZombieLayer;
        targetsInRange = Physics2D.OverlapCircleAll(transform.position, range, zombieMask);
        targetsLOS = new bool[targetsInRange.Length];

        int losMask = (1 << LayerManager.WallLayer) | (1 << LayerManager.BarricadeLayer);
        target = null;
        float shortestLength = Mathf.Infinity;
        for (int i = 0; i < targetsInRange.Length; i++)
        {
            Collider2D c = targetsInRange[i];
            Transform t = c.transform;
            Vector2 rayDirection = t.position - transform.position;
            float length = rayDirection.sqrMagnitude;  // Only comparing
            if (length < shortestLength)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, range, losMask);
                bool lineOfSight = hit.transform == null;
                targetsLOS[i] = lineOfSight;

                if (lineOfSight)
                {
                    target = t;
                    shortestLength = length;
                }
            }
            
        }

        if (target != null)
        {
            Vector2 lookingAt = target.position;
            Vector2 pos = transform.position;
            float angle = Vector2.SignedAngle(Vector2.up, lookingAt - pos);
            plate.rotation = Quaternion.Euler(0f, 0f, angle+90f);

            if (autofire) TryShoot();
        }

        
    }
    void OnTransformChildrenChanged()
    {
        AttachedChanged();
    }
    private void AttachedChanged()
    {
        childGuns = plate.GetComponentsInChildren<Gun>();

        foreach (Transform t in plate.GetComponentsInChildren<Transform>())
        {
            t.localPosition = Vector2.zero;
            t.localRotation = Quaternion.identity;
        }
        foreach (Drop drop in plate.GetComponentsInChildren<Drop>())
        {
            drop.Active = false;
        }
        foreach (Collider2D collider in plate.GetComponentsInChildren<Collider2D>())
        {
            collider.enabled = false;
        }
    }
    private void TryShoot()
    {
        foreach (Gun gun in childGuns)
        {
            if (gun.DelayOver) gun.Shoot();
        }
    }

    public void Interact(Interactor interactor)
    {
        Hotbar hb = interactor.GetComponent<Hotbar>();
        int index = hb.ChosenIndex;

        InventoryObject swappingInv = interactor.GetComponent<PlayerInventoryAPI>().Main;
        if (gunInventory.GetSlotItem(0) == null && swappingInv.GetSlotItem(0) == null)
        {
            // Pick up turret
            Drop d = GetComponent<Drop>();
            swappingInv.Set(index, d.Item, 1);
            GetComponent<Destroyable>().Destroy();
        } else
        {
            // Swap items
            InventoryExtensions.Swap(gunInventory, 0, swappingInv, index);
        }
        
    }
    public void StateChanged(InventoryObject inv)
    {
        if (child != null)
            child.GetComponent<Destroyable>().Destroy();
        Item item = gunInventory.GetSlotItem(0);
        if (item == null)
        {
            child = null;
        }
        else
        {
            GameObject go = EzPools.Instance.Depool(item.Prefab);
            child = go.transform;
            child.SetParent(plate);
        }
        AttachedChanged();
    }
}
