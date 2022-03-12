using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzPools;

public class ItemDropper : MonoBehaviour, IItemReceiver
{
    [SerializeField] private float dropForce;

    public void Add(Item item, int quantity)
    {
        Drop(item, quantity);
    }

    public Drop Drop(Item item, int quantity)
    {
        GameObject prefab = item.Prefab;
        GameObject obj = Manager.Instance.Depool(prefab);
        Drop drop = obj.GetComponent<Drop>();
        if (drop == null)
        {
            Debug.LogWarning(obj + " does not have Drop component");
        } else
        {
            drop.Active = true;
            drop.Item = item;
            drop.Quantity = quantity;
            drop.PickupDelay = 1f;
            drop.transform.SetParent(null);

            Vector2 pos = transform.position;
            drop.transform.position = pos;
            Vector2 mousePos = UI.Instance.RectCalculator.ScreenPointToWorld(Input.mousePosition);
            Vector2 force = (mousePos - pos)*dropForce;
            Rigidbody2D rb = drop.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = force;
        }
        return drop;
    }
}
