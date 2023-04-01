using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="SO/Wallet")]
public class Wallet : ScriptableObject
{
    public int Shitcoin { get => shitcoin; set { Set(value);  } }
    [SerializeField] private int shitcoin;
    private HashSet<IWalletListener> listeners = new();

    public void AddListener(IWalletListener listener)
    {
        listeners.Add(listener);
        Set(shitcoin);
    }
    public void RemoveListener(IWalletListener listener)
    {
        listeners.Remove(listener);
    }
    private void Set(int value)
    {
        shitcoin = value;
        foreach (IWalletListener listener in listeners)
        {
            listener.WalletChanged(shitcoin, value);
        }
    }
}
