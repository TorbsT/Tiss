using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Miner : MonoBehaviour, IBatteryListener, IHPListener, IWalletListener
{
    public Wallet Stored { get => stored; }
    public float Range { get => range; set { range = value; } }
    public float Effratio { get => effratio; set { effratio = value; FireStateChanged(); } }
    public float RealEffratio { get { if (powered) return effratio; return 0f; } }
    public bool Powered => powered;

    [Header("STATE")]
    [SerializeField] private Wallet stored;
    [SerializeField, Range(0f, 10f)] private float range;
    //[SerializeField, Range(0.1f, 10f)] private float speed;  // Production per second

    [Header("DEBUG")]
    [SerializeField] private float effratio;
    private bool powered;

    private HashSet<IMinerListener> listeners = new();

    private void Awake()
    {
        stored = ScriptableObject.CreateInstance<Wallet>();
        stored.AddListener(this);
    }
    private void OnEnable()
    {
        stored.Shitcoin = 0;
        GetComponent<HP>().Set(100f);
        MinerSystem.Instance.Track(this);
    }
    private void OnDisable()
    {
        MinerSystem.Instance.Untrack(this);
    }
    private void Update()
    {

    }
    public void AddListener(IMinerListener listener)
    {
        listeners.Add(listener);
        listener.MinerChanged(this);
    }
    public void RemoveListener(IMinerListener listener)
    {
        listeners.Remove(listener);
    }
    public void NewCharge(int oldCharge, int newCharge)
    {
        Target t = GetComponent<Target>();
        powered = newCharge > 0;
        if (powered) t.SetDiscoverability(Target.Discoverability.discoverable);
        else t.SetDiscoverability(Target.Discoverability.hidden);
        FireStateChanged();
    }
    public void NewHP(float oldHP, float newHP)
    {
        if (newHP <= 0f)
        {
            GetComponent<Destroyable>().Destroy();
        }
    }
    private void FireStateChanged()
    {
        foreach (IMinerListener listener in listeners)
        {
            listener.MinerChanged(this);
        }
    }
    void IWalletListener.WalletChanged(int oldValue, int newValue)
    {
        FireStateChanged();
    }
}
