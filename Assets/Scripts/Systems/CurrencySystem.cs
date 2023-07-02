using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    public class CurrencySystem : MonoBehaviour
    {
        public static CurrencySystem Instance { get; private set; }

        public event Action<int> BalanceChanged;
        public int Balance {
            get => int.Parse(balanceText.text);
            set
            {
                balanceText.text = value.ToString();
                BalanceChanged?.Invoke(value);
            }
        }
        [SerializeField] private TextMeshProUGUI balanceText;
        [SerializeField] private string singleCurrencyName = "Soul shard";
        [SerializeField] private string multipleCurrencyName = "Soul shards";

        public string GetCurrencied(int value)
        {
            if (Mathf.Abs(value) == 1)
                return $"{value} {singleCurrencyName}";
            return $"{value} {multipleCurrencyName}";
        }
        private void Awake()
        {
            Balance = 0;
            Instance = this;
        }
    }
}