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
    private Hotbar observedHotbar;
    [SerializeField] private Item chosenItem;
    [SerializeField] private State state;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform gunSocket;
    [SerializeField] private GameObject holsteredGO;

    // Cached
    private Gun holsteredGun;
    private bool holdingPlaceable;

    void Start()
    {
        FindHotbar();
    }
    void Update()
    {
        KeyCode m1 = KeyCode.Mouse0;
        if (holsteredGun != null)
        {
            if (holsteredGun.CanShoot)
            {
                if (holsteredGun.FireMode == Gun.Mode.semiauto && Input.GetKeyDown(m1)) holsteredGun.Shoot();
                else if (holsteredGun.FireMode == Gun.Mode.auto && Input.GetKey(m1)) holsteredGun.Shoot();
            }
        }
        if (holdingPlaceable)
        {
            if (Input.GetKeyDown(m1))
            {
                GetComponent<ItemPlacer>().Place();
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


        holdingPlaceable = false;
        if (chosenItem != null)
        {
            GameObject prefab = chosenItem.Prefab;
            if (prefab.GetComponent<Holsterable>() != null)
            {
                holsteredGO = EzPools.Instance.Depool(prefab);
                Drop drop = holsteredGO.GetComponent<Drop>();
                if (drop != null) drop.Active = false;
                holsteredGun = holsteredGO.GetComponent<Gun>();
                Collider2D collider = holsteredGO.GetComponent<Collider2D>();
                if (collider != null) collider.enabled = false;
                Rigidbody2D rb = holsteredGO.GetComponent<Rigidbody2D>();
                if (rb != null) rb.simulated = false;
                holsteredGO.GetComponent<Holsterable>().Holster(this);
            }
            if (prefab.GetComponent<Placeable>() != null)
            {
                holdingPlaceable = true;
                GetComponent<ItemPlacer>().PrefabToPreview = prefab;
            }
        }
        if (!holdingPlaceable) GetComponent<ItemPlacer>().PrefabToPreview = null;

    }
    public void SetAnimation(State newState)
    {
        
        state = newState;
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
}
