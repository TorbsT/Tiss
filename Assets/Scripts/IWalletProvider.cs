using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWalletProvider
{
    Wallet Wallet { get; }
}
