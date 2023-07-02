using Assets.Scripts.Components;
using Assets.Scripts.ScriptableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    internal class ChestSystem : MonoBehaviour, IInteractionHandler
    {
        public static ChestSystem Instance { get; private set; }

        [field: SerializeField] public Upgrade UpgradePrefab { get; private set; }
        [field: SerializeField] public List<UpgradeObject> Upgrades
        { get; private set; } = new();
        [field: SerializeField] public int LootCount { get; set; } = -1;
        [field: SerializeField] public float LootRarity { get; set; } = -1;
        [field: SerializeField] public Vector2 Distances { get; set; } = Vector2.one * 0.1f;
        [field: SerializeField, Range(0f, 1f)] public float ChestChance { get; set; } = 0.35f;

        public void Select(GameObject go)
        {
            if (go == null) return;
            Chest chest = go.GetComponent<Chest>();
            if (chest != null)
            {
                SelectChest(chest);
                return;
            }
            Upgrade upgrade = go.GetComponent<Upgrade>();
            if (upgrade != null)
                SelectUpgrade(upgrade);
        }
        public Tooltip.TooltipData Hover(GameObject go)
        {
            if (go == null) return null;
            Upgrade upgrade = go.GetComponent<Upgrade>();
            if (upgrade != null)
            {
                UpgradeObject up = upgrade.Object;
                string adjustedDescription =
                    InventorySystem.Instance.GetLevelAdjustedDesc(
                        up.Description,
                    InventorySystem.Instance.GetUpgradeLevel(up) + 1);
                return new(up.Name,
                    adjustedDescription,
                    up.Rarity.Color,
                    up.Rarity.name);
            }
            return null;
        }
        private void Awake()
        {
            Instance = this;
            Upgrades = Resources.LoadAll<UpgradeObject>("").ToList();
        }
        private void OnEnable()
        {
            ChunkLoader.Instance.LoadedDelta += LoadDelta;
        }
        private void OnDisable()
        {
            ChunkLoader.Instance.LoadedDelta -= LoadDelta;
        }
        private void SelectChest(Chest chest)
        {
            chest.SpriteRenderer.sprite = chest.OpenSprite;
            chest.GetComponent<Interactable>().enabled = false;

            List<UpgradeObject> upgradesLeft = new();
            foreach (var upgrade in Upgrades)
                upgradesLeft.Add(upgrade);

            HashSet<UpgradeObject> chosenUpgrades = new();
            while (
                upgradesLeft.Count > 0 &&
                chosenUpgrades.Count < LootCount)
            {
                int random = UnityEngine.Random.Range(0, upgradesLeft.Count);
                UpgradeObject upgrade = upgradesLeft[random];
                chosenUpgrades.Add(upgrade);
                upgradesLeft.RemoveAt(random);
            }

            List<Upgrade> upgradeComponents = new();
            float i = -(chosenUpgrades.Count-1) / 2f;
            foreach (var upgradeObject in chosenUpgrades)
            {
                Upgrade upgrade = PoolSystem.Depool(UpgradePrefab.gameObject)
                    .GetComponent<Upgrade>();
                upgrade.Object = upgradeObject;
                VFXSystem.Instance.Play(VFXSystem.Instance.CommonUpgrade, upgrade.transform);

                SpriteRenderer renderer = upgrade.GetComponent<SpriteRenderer>();
                if (upgradeObject.Sprite != null)
                    renderer.sprite = upgradeObject.Sprite;
                renderer.size = Vector2.one * Distances.x;
                renderer.GetComponent<BoxCollider2D>().size = Vector2.one * Distances.x;

                upgrade.transform.SetParent(chest.transform);
                upgrade.transform.localPosition =
                    new Vector2(i * Distances.x, Distances.y);
                upgradeComponents.Add(upgrade);
                i++;
            }
        }
        private void SelectUpgrade(Upgrade upgrade)
        {
            Chest chest = upgrade.GetComponentInParent<Chest>();
            ConsoleActor.Instance.Execute(upgrade.Object.Commands);
            foreach (var child in chest.GetComponentsInChildren<Upgrade>())
            {
                PoolSystem.Enpool(child.gameObject);
            }
            InventorySystem.Instance.AddUpgrade(upgrade);  // todo event?
            chest.CachedAnimator.SetTrigger("fade");
            chest.DelayedDisappear();
        }
        private void LoadDelta(Dictionary<Vector2Int, GameObject> loaded)
        {
            foreach (var loc in loaded.Keys)
            {
                var go = loaded[loc];
                bool show = true;
                show &= UnityEngine.Random.Range(0f, 1f) < ChestChance;
                show &= Mathf.Abs(loc.x) + Mathf.Abs(loc.y) > 1;

                Chest chest = go.GetComponentInChildren<Chest>(true);
                chest.gameObject.SetActive(show);
            }
        }
    }
}
