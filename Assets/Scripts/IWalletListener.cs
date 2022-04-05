using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWalletListener
{
    void WalletChanged(int oldValue, int newValue);
}
