using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSucker : MonoBehaviour
{
    [SerializeField] private bool gizmos;
    [SerializeField] private float suckRange;
    [SerializeField] private float suckForce;
    [SerializeField] private float pickupRange;
    [SerializeField] private AnimationCurve suckCurve;
    private PlayerInventoryAPI api;
    private Collider2D[] hits;
    private HashSet<Collider2D> validHits = new();

    private void OnDrawGizmosSelected()
    {
        if (!gizmos) return;
        Vector3 pos = transform.position;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(pos, suckRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(pos, pickupRange);

        if (hits != null)
        {
            Gizmos.color = Color.magenta;
            foreach (Collider2D col in hits)
            {
                if (col != null)
                Gizmos.DrawSphere(col.transform.position, 0.5f);
            }
            Gizmos.color = Color.red;
            foreach (Collider2D col in validHits)
            {
                if (col != null)
                Gizmos.DrawSphere(col.transform.position, 0.5f);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Use for shitcoin miners https://www.xarg.org/2016/07/calculate-the-intersection-area-of-two-circles/
        int layerMask = 1 << LayerManager.DroppedItemLayer;
        hits = Physics2D.OverlapCircleAll(transform.position, suckRange, layerMask);
        validHits = new();
        foreach (Collider2D hit in hits)
        {
            Drop drop = hit.GetComponent<Drop>();
            if (drop == null) Debug.LogError("Error: " + hit + " is in droppeditemlayer but has no IDrop component");
            if (!drop.Active) continue;

            validHits.Add(hit);
            Vector2 distance = transform.position - hit.transform.position;
            float magnitude = distance.magnitude;

            if (drop.PickupDelay <= 0f && magnitude < pickupRange)
            {
                Item item = drop.Item;
                int quantity = drop.Quantity;
                int remaining = InventoryExtensions.QuickAdd(PlayerInventoryAPI.Instance.Main, item, quantity);
                int used = quantity - remaining;
                drop.Pickup(used);
            } else
            {
                float force = suckCurve.Evaluate(magnitude / suckRange)*suckForce;
                drop.Pull(distance, force);
            }
        }
    }
}
