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
    public Interactable CurrentHover => currentHover;

    [SerializeField] private float hoverGetRange;
    [SerializeField] private float hoverLoseRange;
    [SerializeField] private float verifyWaitTime;
    [SerializeField] private float findWaitTime;

    private Interactable currentHover;
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

        if (currentHover != null && currentHover.isActiveAndEnabled)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                currentHover.Interact(this);
                Unhover();
            }
        }
    }
    private void Verify()
    {
        timeWaited = 0f;
        if (OutOfRange() || !currentHover.isActiveAndEnabled) Unhover();
    }
    private bool OutOfRange()
    {
        float distance = (currentHover.transform.position - transform.position).magnitude;
        return distance > hoverLoseRange;
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
        Vector2 pos2 = transform.position;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(pos2, hoverGetRange);
        foreach (Collider2D hitCollider in hitColliders)
        {
            Interactable interactable = hitCollider.GetComponent<Interactable>();
            if (interactable == null || !interactable.Active) continue;
            float distance = (interactable.transform.position - transform.position).magnitude;
            if (distance > hoverGetRange) continue;  // Takes colliders out of the equation
            currentHover = interactable;
            currentHover.Hover(this);
            state = State.hovering;
            break;
        }
    }
}
