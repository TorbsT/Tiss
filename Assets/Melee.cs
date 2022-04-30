using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    [SerializeField] private bool gizmos;
    [SerializeField] private Animator animator;
    [SerializeField, Range(0f, 100f)] private float range = 100f;
    [SerializeField, Range(0f, 100f)] private float knockback = 100f;

    private Vector2 origin;
    private Vector2 aim;
    private Vector2 hitPos;

    private void OnDrawGizmos()
    {
        if (gizmos)
        {
            Gizmos.color = Color.blue;

            Gizmos.DrawLine(origin, (aim-origin).normalized*range+origin);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(origin, hitPos);
        }
    }

    public void Shoot()
    {
        animator.SetTrigger("Shoot");
        origin = Player.Instance.transform.position;
        aim = UI.Instance.RectCalculator.ScreenPointToWorld(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(origin, aim-origin, range, 1 << LayerManager.ZombieLayer);
        if (hit.collider != null)
        {
            hitPos = hit.transform.position;
            hit.collider.attachedRigidbody.AddForce((hitPos - origin)*knockback);
            hit.collider.GetComponent<HP>()?.Decrease(20);
        }
    }
}
