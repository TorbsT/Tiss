using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzPools;

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

    void Start()
    {
        FindHotbar();
    }
    void Update()
    {
        if (holsteredGO == null) return;
        if (holsteredGun != null)
        {
            if (holsteredGun.CanShoot)
            {
                KeyCode fireCode = KeyCode.Mouse0;
                if (holsteredGun.FireMode == Gun.Mode.semiauto && Input.GetKeyDown(fireCode)) holsteredGun.Shoot();
                else if (holsteredGun.FireMode == Gun.Mode.auto && Input.GetKey(fireCode)) holsteredGun.Shoot();
            }
        }
    }

    public void StateChanged(Hotbar hotbar)
    {
        chosenItem = hotbar.ChosenItem;
        if (holsteredGO != null)
        {
            holsteredGO.GetComponent<IHolsterable>().Unholster();
        }
        holsteredGun = null;

        bool canHolster = chosenItem != null;
        if (!canHolster) return;
        GameObject prefab = chosenItem.Prefab;
        canHolster = prefab.GetComponent<IHolsterable>() != null;
        if (canHolster)
        {
            holsteredGO = Manager.Instance.Depool(prefab);
            Drop drop = holsteredGO.GetComponent<Drop>();
            if (drop != null) drop.Active = false;
            holsteredGun = holsteredGO.GetComponent<Gun>();
            Collider2D collider = holsteredGO.GetComponent<Collider2D>();
            if (collider != null) collider.enabled = false;
            Rigidbody2D rb = holsteredGO.GetComponent<Rigidbody2D>();
            if (rb != null) rb.simulated = false;
            holsteredGO.GetComponent<IHolsterable>().Holster(this);
        }
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
