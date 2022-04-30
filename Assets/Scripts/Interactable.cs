using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pools;

public class Interactable : MonoBehaviour
{
    public bool Active { get => active; set { active = value; } }
    private HashSet<IInteractableListener> listeners = new();
    private Tooltip tooltip;
    [SerializeField] private bool active = true;
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
        tooltip.Offset = Vector2.up*2f;
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
    public void AddListener(IInteractableListener listener)
    {
        listeners.Add(listener);
    }
    public void RemoveListener(IInteractableListener listener)
    {
        listeners.Remove(listener);
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
}
