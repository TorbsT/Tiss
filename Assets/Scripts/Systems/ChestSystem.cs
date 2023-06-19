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
    internal class ChestSystem : MonoBehaviour
    {
        public static ChestSystem Instance { get; private set; }

        [field: SerializeField] public Upgrade UpgradePrefab { get; private set; }
        [field: SerializeField] public List<UpgradeObject> Upgrades
        { get; private set; } = new();
        [field: SerializeField] public int LootCount { get; set; } = 3;
        [field: SerializeField] public Vector2 Distances { get; set; } = Vector2.one * 0.1f;

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
            int i = -chosenUpgrades.Count / 2;
            foreach (var upgradeObject in chosenUpgrades)
            {
                Upgrade upgrade = PoolSystem.Depool(UpgradePrefab.gameObject)
                    .GetComponent<Upgrade>();
                upgrade.Object = upgradeObject;
                VFXSystem.Instance.Play(VFXSystem.Instance.CommonUpgrade, upgrade.transform);
                if (upgradeObject.Sprite != null)
                    upgrade.GetComponent<SpriteRenderer>().sprite = upgradeObject.Sprite;
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
        }
        private void Awake()
        {
            Instance = this;
        }
    }
}
