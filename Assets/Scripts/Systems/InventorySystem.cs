using Assets.Scripts.Components;
using Assets.Scripts.ScriptableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorbuTils.Anime;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    internal class InventorySystem : MonoBehaviour, IInteractionHandler
    {
        public static InventorySystem Instance { get; private set; }
        
        [field: SerializeField] public RectTransform Inventory { get; private set; }
        [field: SerializeField] public GameObject ItemPrefab { get; private set; }
        [field: SerializeField] public GameObject SlotPrefab { get; private set; }
        [field: SerializeField, Range(0, 64)] public int SlotCount { get; private set; } = 16;
        [field: SerializeField] public AnimObject AnimObject { get; private set; }
        private bool visible = false;

        public string GetLevelAdjustedDesc(string desc, int level)
        {
            List<string> newDesc = new();
            string[] opens = desc.Split("{");
            newDesc.Add(opens[0]);
            for (int i = 1; i < opens.Length; i++)
            {
                string part = opens[i];
                // part contains a } somewhere
                int endIndex = part.IndexOf("}");
                string valueString = part[..endIndex];
                string restString = part[(endIndex + 1)..];

                newDesc.Add(GetLevelAdjustedValueString(valueString, level));
                newDesc.Add(restString);
            }
            return string.Join("", newDesc);
        }
        public string GetLevelAdjustedValueString(string input, int level)
        {
            bool negative = input.StartsWith("-");
            bool percent = input.EndsWith("%");
            float value = GetLevelAdjustedValue(input, level);

            if (!percent)
                return negative ? $"{value}" : $"+{value}";

            float realValue = negative ? 1f - value : value - 1f;
            int percentValue = Mathf.RoundToInt(realValue * 100f);
            return negative ? $"-{percentValue}%" : $"+{percentValue}%";
        }
        public float GetLevelAdjustedValue(string input, int level)
        {
            if (level == 0) return 0f;
            bool negative = input.StartsWith("-");
            bool percentage = input.EndsWith("%");
            float value = float.Parse(input.Replace("%", "").Replace("-", "").Replace("+", ""));

            if (!percentage)
            {
                if (negative)
                    return -value * level;
                else
                    return value * level;
            }
            float decimalValue = value / 100f;
            float prePowerValue = negative ? 1f - decimalValue : 1f + decimalValue;
            float finalValue = Mathf.Pow(prePowerValue, level);
            return finalValue;
        }
        public int GetUpgradeLevel(UpgradeObject obj)
        {
            foreach (var item in Inventory.GetComponentsInChildren<Item>())
                if (item.UpgradeObject == obj)
                    return item.Level;
            return 0;
        }
        public bool CanAddUpgrade(Upgrade upgrade)
            => GetChosenIndex(upgrade).HasValue;
        public void AddUpgrade(Upgrade upgrade)
        {
            if (!visible)
                SlideIn();

            int? index = GetChosenIndex(upgrade);
            if (!index.HasValue)
            {
                Debug.LogWarning("No available slots");
                return;
            }

            UpgradeObject obj = upgrade.Object;

            GameObject go = Inventory.GetChild(index.Value).gameObject;
            Item item = go.GetComponent<Item>();
            if (item == null)
            {
                PoolSystem.Enpool(go);
                go.transform.SetParent(null);  // Prevent weird stuff
                go = PoolSystem.Depool(ItemPrefab);
                go.transform.SetParent(Inventory);
                go.transform.SetSiblingIndex(index.Value);
                item = go.GetComponent<Item>();
                item.Image.sprite = obj.Sprite;
                item.UpgradeObject = obj;
                item.Level = 0;
                item.LevelBG.color = obj.Rarity.Color;
            }

            item.Level += 1;
        }
        public Tooltip.TooltipData Hover(GameObject go)
        {
            if (go == null) return null;
            Item item = go.GetComponent<Item>();
            if (item == null) return null;

            UpgradeObject obj = item.UpgradeObject;
            string adjustedDescription = GetLevelAdjustedDesc(obj.Description,
                GetUpgradeLevel(obj));
            Tooltip.TooltipData data = new(
                obj.Name,
                adjustedDescription,
                obj.Rarity.Color,
                obj.Rarity.name);
            return data;
        }
        public void Select(GameObject go)
        {
            return;
        }

        private void Awake()
        {
            Instance = this;
            foreach (var preItem in Inventory.GetComponentsInChildren<RectTransform>())
                if (preItem.parent == Inventory)
                    Destroy(preItem.gameObject);
        }
        private void Start()
        {
            for (int i = 0; i < SlotCount; i++)
            {
                GameObject go = PoolSystem.Depool(SlotPrefab);
                go.transform.SetParent(Inventory);
            }
            Inventory.anchoredPosition = Vector2.left*Inventory.rect.width;
        }

        private int? GetChosenIndex(Upgrade upgrade)
        {
            // First, check if level should be increased
            foreach (var itm in GetComponentsInChildren<Item>())
            {
                if (upgrade.Object == itm.UpgradeObject)
                {  // Just level up that item
                    return itm.transform.GetSiblingIndex();
                }
            }

            // Try finding empty slot
            for (int i = 0; i < SlotCount; i++)
            {
                var slot = Inventory.GetChild(i);
                if (slot.GetComponent<Item>() != null) continue;
                return slot.GetSiblingIndex();
            }

            // full inventory
            return null;
        }
        private void SlideIn()
        {
            visible = true;
            Anim<Vector2> slideAnim = new()
            {
                Action = (value) => { Inventory.anchoredPosition = value; },
                StartValue = Inventory.anchoredPosition,
                EndValue = Vector2.zero,
                Duration = AnimObject.Duration,
                Curve = AnimObject.Curve,
            };
            slideAnim.Start();
        }
    }
}
