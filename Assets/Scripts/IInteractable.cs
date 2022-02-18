using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    Vector3 Position { get; }
    bool CanHover();
    bool CanInteract();
    void Hover();
    void Unhover();
    void Interact();
}
