using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    Vector3 Position { get; }
    bool CanHover(Interactor interactor);
    bool CanInteract(Interactor interactor);
    void Hover(Interactor interactor);
    void Unhover(Interactor interactor);
    void Interact(Interactor interactor);
}
