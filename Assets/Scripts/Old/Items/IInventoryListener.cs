using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryListener
{
    void StateChanged(InventoryObject obj);
}
