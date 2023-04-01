using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pools;

public class Interactable : MonoBehaviour
{
    public bool Active { get => active; set { active = value; } }
    public float OpenRange => openRange;
    public float CloseRange => closeRange;
    public Vector2 Offset => offset;

    [SerializeField] private bool active = true;
    [SerializeField, Range(1f,10f)] private float openRange = 5f;
    [SerializeField, Range(2f, 15f)] private float closeRange = 10f;
    [SerializeField] private Vector2 offset = Vector2.up*1f;

    //private Tooltip tooltip;

    /*
    public void Hover(Interactor interactor)
    {
        if (!active)
        {
            Debug.LogWarning(this + " is not active, do not call Hover");
        }
        if (tooltip != null)
        {
            Debug.LogWarning("Already hovering " + this);
            return;
        }
        tooltip = TooltipPool.Instance.Depool();
        tooltip.KeyCode = KeyCode.F;
        tooltip.transform.SetParent(UI.Instance.TooltipTransform, false);
        tooltip.TransformToFollow = transform;
        tooltip.Offset = offset;
    }

    public void Unhover(Interactor interactor)
    {
        if (!active)
        {
            Debug.LogWarning(this + " is not active, do not call Unhover");
        }
        TooltipPool.Instance.Enpool(tooltip);
        tooltip = null;
    }
    public void Interact(Interactor interactor)
    {
        if (!active)
        {
            Debug.LogWarning(this + " is not active, do not call Interact");
        }
        foreach (IInteractableListener listener in listeners)
        {
            listener.Interact(interactor);
        }
    }
    */
}
