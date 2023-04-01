using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hands : MonoBehaviour, IHotbarListener
{
    public enum State
    {
        none,
        longGun
    }

    public Transform GunSocket => gunSocket;

    // Start is called before the first frame update
    [Header("ASSIGN")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform gunSocket;
    [SerializeField] private float throwForce;

    [Header("DEBUG")]
    [SerializeField] private Item chosenItem;
    [SerializeField] private State state;
    [SerializeField] private GameObject holsteredGO;

    private Hotbar observedHotbar;
    // Cached
    private Weapon holsteredWeapon;
    private Melee holsteredMelee;
    private Gun holsteredGun;
    private bool holdingBomb;
    private bool holdingPlaceable;

    void Start()
    {
        FindHotbar();
    }
    void Update()
    {
        KeyCode m1 = KeyCode.Mouse0;
        if (holsteredWeapon != null)
        {
            if (holsteredWeapon.DelayOver)
            {
                if (holsteredWeapon.FireMode == Weapon.Mode.semiauto && Input.GetKeyDown(m1)) TryShoot();
                else if (holsteredWeapon.FireMode == Weapon.Mode.auto && Input.GetKey(m1)) TryShoot();
            }
        }
        else if (holdingPlaceable)
        {
            if (Input.GetKeyDown(m1) && GetComponent<ItemPlacer>().CanPlace)
            {
                GetComponent<ItemPlacer>().Place();
                InventoryExtensions.QuickRemove(GetComponent<PlayerInventoryAPI>().Main, GetComponent<Hotbar>().ChosenIndex, 1);
            }
        } else if (holdingBomb)
        {
            if (Input.GetKeyDown(m1))
            {
                ThrowBomb();
                InventoryExtensions.QuickRemove(GetComponent<PlayerInventoryAPI>().Main, GetComponent<Hotbar>().ChosenIndex, 1);
            }
        }
    }
    public void StateChanged(Hotbar hotbar)
    {
        chosenItem = hotbar.ChosenItem;
        if (holsteredGO != null)
        {
            holsteredGO.GetComponent<Destroyable>().Destroy();
            holsteredGO = null;
        }
        holsteredGun = null;
        holsteredMelee = null;
        holsteredWeapon = null;

        holdingPlaceable = false;
        holdingBomb = false;
        if (chosenItem != null)
        {
            GameObject prefab = chosenItem.Prefab;
            if (prefab.GetComponent<Holsterable>() != null)
            {
                holsteredGO = EzPools.Instance.Depool(prefab);
                Drop drop = holsteredGO.GetComponent<Drop>();
                if (drop != null) drop.Active = false;
                holsteredWeapon = holsteredGO.GetComponent<Weapon>();
                holsteredMelee = holsteredGO.GetComponent<Melee>();
                holsteredGun = holsteredGO.GetComponent<Gun>();
                //Collider2D collider = holsteredGO.GetComponent<Collider2D>();
                //if (collider != null) collider.enabled = false;
                //Rigidbody2D rb = holsteredGO.GetComponent<Rigidbody2D>();
                //if (rb != null) rb.simulated = false;
                holsteredGO.GetComponent<Holsterable>().Holster(this);
            }
            if (prefab.GetComponent<Placeable>() != null)
            {
                holdingPlaceable = true;
                GetComponent<ItemPlacer>().PrefabToPreview = prefab;
            }
            if (prefab.GetComponent<RewindBomb>() != null)
            {
                holdingBomb = true;
            }
        }
        if (!holdingPlaceable) GetComponent<ItemPlacer>().PrefabToPreview = null;

    }

    private void ThrowBomb()
    {
        GameObject go = EzPools.Instance.Depool(chosenItem.Prefab);
        Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
        Vector2 pos = transform.position;

        go.transform.position = pos;
        rb.AddForce(100f * throwForce * (GetComponent<LookAt>().LookingAt - pos).normalized);
    }
    private void TryShoot()
    {
        bool canShoot = true;
        if (holsteredGun != null)
        {
            // check if there is ammo
            InventoryObject inv = GetComponent<PlayerInventoryAPI>().Main;
            Item ammo = holsteredGun.GunSO.Ammo;
            int usedAmmo = holsteredGun.GunSO.AmmoPerBurst;
            canShoot = InventoryExtensions.CanQuickRemove(inv, ammo, usedAmmo);
            if (canShoot)
            {
                InventoryExtensions.QuickRemove(inv, ammo, usedAmmo);
            }
        }

        if (canShoot) holsteredWeapon.Shoot();
    }
    private void FindHotbar()
    {
        if (observedHotbar != null)
        {
            observedHotbar.RemoveListener(this);
        }

        observedHotbar = GetComponent<Hotbar>();
        if (observedHotbar == null)
        {
            Invoke(nameof(FindHotbar), 1f);
        }
        else
        {
            GetComponent<Hotbar>().AddListener(this);
        }
    }

    void OnDestroy()
    {
        observedHotbar.RemoveListener(this);
    }
}
