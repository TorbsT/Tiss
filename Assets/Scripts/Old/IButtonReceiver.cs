using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IButtonReceiver
{
    void Click(UIItemSlot slot, UIInventory uiInventory);
}
