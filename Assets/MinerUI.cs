using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MinerUI : MonoBehaviour, IMinerListener
{
    public static MinerUI Instance { get; private set; }

    [Header("CONFIG")]
    [SerializeField, Range(1f, 5f)] private float maxDistance;
    [SerializeField] private Gradient efficiencyColors;

    [Header("ASSIGN")]
    [SerializeField] private TextMeshProUGUI storedField;
    [SerializeField] private TextMeshProUGUI efficiencyField;
    [SerializeField] private Image efficiencyMeter;
    [SerializeField] private GameObject efficiencyTutorial;
    [SerializeField] private GameObject needsPowerGO;
    [SerializeField] private GameObject poweredGO;

    [Header("DEBUG")]
    [SerializeField] private Miner miner;
    [SerializeField] private Interactor interactor;
    private void Awake()
    {
        Instance = this;
        Close();
    }
    private void Update()
    {
        if (miner == null) return;
        if ((interactor.transform.position-miner.transform.position).magnitude > maxDistance)
        {
            Close();
        }
    }
    public void Open(Miner miner, Interactor interactor)
    {
        bool justClosing = this.miner != null;
        Close();
        if (!justClosing)
        {
            this.miner = miner;
            this.interactor = interactor;
            gameObject.SetActive(true);
            miner.AddListener(this);
        }

    }
    public void Close()
    {
        if (this.miner != null) this.miner.RemoveListener(this);
        this.miner = null;
        this.interactor = null;
        gameObject.SetActive(false);
    }

    public void Move()
    {
        InventoryObject main = interactor.GetComponent<PlayerInventoryAPI>().Main;
        Item item = miner.GetComponent<Drop>().Item;
        if (InventoryExtensions.QuickAdd(main, item, 1) == 0)
        {
            Collect();
            miner.GetComponent<Destroyable>().Destroy();
            Close();
        }
    }
    public void Collect()
    {
        Wallet toWallet = interactor.GetComponent<IWalletProvider>().Wallet;
        toWallet.Shitcoin += miner.Stored.Shitcoin;
        miner.Stored.Shitcoin = 0;
    }
    public void MinerChanged(Miner miner)
    {
        storedField.text = "$"+miner.Stored.Shitcoin;
        int percent = Mathf.CeilToInt(miner.Effratio * 100f);
        efficiencyField.text =  percent+"%";
        efficiencyTutorial.SetActive(percent < 100);

        Color color = efficiencyColors.Evaluate(miner.Effratio);
        efficiencyMeter.color = color;
        efficiencyMeter.fillAmount = miner.Effratio;

        bool powered = miner.GetComponent<Battery>().Power > 0;
        poweredGO.SetActive(powered);
        needsPowerGO.SetActive(!powered);
    }
}
