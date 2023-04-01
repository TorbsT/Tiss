using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShitcoinTracker : MonoBehaviour, IWalletListener
{
    private TextMeshProUGUI field;

    // Start is called before the first frame update
    void Awake()
    {
        field = GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        PlayerInventoryAPI.Instance.Wallet.AddListener(this);
    }
    public void WalletChanged(int oldValue, int newValue)
    {
        field.text = "$" + newValue;
    }
}
