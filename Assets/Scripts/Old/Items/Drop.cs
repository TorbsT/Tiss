using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Drop : MonoBehaviour
{
    public float PickupDelay { get => pickupDelay; set { pickupDelay = value; } }
    public bool Active { get => active; set { active = value; } }
    public Item Item { get => item; set { item = value; } }
    public int Quantity { get => quantity; set { quantity = value; } }

    [SerializeField] private bool active = true;
    [SerializeField] private Item item;
    [SerializeField] private int quantity = 1;
    [SerializeField] private float pickupDelay;

    private void Update()
    {
        if (pickupDelay > 0)
        {
            pickupDelay -= Time.deltaTime;
        } else
        {
            pickupDelay = 0f;
        }
        
    }
    public void Pull(Vector2 direction, float force)
    {
        if (pickupDelay > 0) return;
        if (!active)
        {
            Debug.LogWarning("Tried pulling " + this + " while it was not a drop");
            return;
        }
        Vector2 delta = direction.normalized * force;
        transform.position += new Vector3(delta.x,delta.y,0f);
    }
    public void Pickup(int quantity)
    {
        if (!active || pickupDelay > 0)
        {
            Debug.LogWarning("Tried pickup on " + this + " while it was not a drop");
            return;
        }

        int result = this.quantity - quantity;
        if (result < 0)
        {
            Debug.LogWarning("Picked up wayy too many " + item + ": had " + this.quantity + ", removed " + quantity);
        }
        this.quantity = result;
        if (this.quantity <= 0)
        {
            GetComponent<Destroyable>().Destroy();
        }
    }

}
