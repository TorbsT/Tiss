using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemReceiver
{
    void Add(Item item, int quantity);
}
