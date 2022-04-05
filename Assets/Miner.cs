using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : MonoBehaviour, IInteractableListener
{
    public Wallet Stored { get => stored; set { stored = value; } }
    public float Range { get => range; set { range = value; } }
    public float Effratio { get => effratio; set { effratio = value; } }
    public int MaxProduction { get => maxProduction; set { maxProduction = value; } }

    [Header("STATE")]
    [SerializeField] private Wallet stored;
    [SerializeField, Range(0f, 100f)] private int maxProduction;
    [SerializeField, Range(0f, 10f)] private float range;
    [SerializeField, Range(0.1f, 10f)] private float speed;  // Production per second

    [Header("DEBUG")]
    [SerializeField] private float effratio;
    [SerializeField] private int available;
    [SerializeField] private float waitPoints;
    private void Awake()
    {
        stored = ScriptableObject.CreateInstance<Wallet>();
        GetComponent<Interactable>().AddListener(this);
    }
    private void OnEnable()
    {
        MinerSystem.Track(this);
        stored.Shitcoin = 0;
    }
    private void OnDisable()
    {
        MinerSystem.Instance.Untrack(this);
    }
    private void Update()
    {
        if (available > 0)
        {
            waitPoints += Time.deltaTime;
            if (waitPoints > 1f/speed)
            {
                waitPoints -= 1f / speed;
                AvailableToStored(1);
            }
        } else
        {
            waitPoints = 0f;
        }
    }
    public void Finish()
    {
        AvailableToStored(available);
    }
    public void AddToAvailable(int amount)
    {
        available += amount;
    }
    public void Interact(Interactor interactor)
    {
        Wallet toWallet = interactor.GetComponent<IWalletProvider>().Wallet;
        toWallet.Shitcoin += stored.Shitcoin;
        stored.Shitcoin = 0;
    }
    private void AvailableToStored(int amount)
    {
        // Can't take more than available
        amount = Mathf.Min(amount, available);
        stored.Shitcoin += amount;
        available -= amount;
    }
}
