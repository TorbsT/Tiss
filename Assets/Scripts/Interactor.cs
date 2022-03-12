using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    private enum State
    {
        none,
        hovering
    }
    public IInteractable CurrentHover => currentHover;

    [SerializeField] private float hoverGetRange;
    [SerializeField] private float hoverLoseRange;
    [SerializeField] private float verifyWaitTime;
    [SerializeField] private float findWaitTime;

    private IInteractable currentHover;
    [SerializeField] private State state;
    [SerializeField] private float timeWaited;
    // Update is called once per frame
    void Update()
    {
        timeWaited += Time.deltaTime;
        if (currentHover == null)
        {
            if (timeWaited > findWaitTime)
            {
                Find();
            }
            
        } else
        {
            if (timeWaited > verifyWaitTime)
            {
                Verify();
                if (currentHover == null) Find();
            }
        }

        if (currentHover != null)
        {
            if (Input.GetKeyDown(KeyCode.F) && currentHover.CanInteract(this))
            {
                currentHover.Interact(this);
                Unhover();
            }
        }
    }
    private bool OutOfRange()
    {
        float distance = (currentHover.Position - transform.position).magnitude;
        return distance > hoverLoseRange;
    }
    private void Verify()
    {
        timeWaited = 0f;
        if (OutOfRange()) Unhover();
    }
    private void Unhover()
    {
        currentHover.Unhover(this);
        currentHover = null;
        state = State.none;
    }
    private void Find()
    {
        timeWaited = 0f;
        Vector3 pos3 = transform.position;
        Vector2 pos2 = new(pos3.x, pos3.y);
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(pos2, hoverGetRange);
        foreach (Collider2D hitCollider in hitColliders)
        {
            IInteractable interactable = hitCollider.GetComponent<IInteractable>();
            if (interactable == null) continue;
            if (!interactable.CanHover(this)) continue;
            currentHover = interactable;
            currentHover.Hover(this);
            state = State.hovering;
            break;
        }
    }
}
