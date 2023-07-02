using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Components
{
    // For the moment only towers
    public class ShopItem : MonoBehaviour
    {
        [field: SerializeField] public GameObject Prefab { get; private set; }
        [field: SerializeField] public Sprite BuildPreviewSprite { get; private set; }
        [field: SerializeField] public Image PriceImage { get; private set; }
        [field: SerializeField] public TextMeshProUGUI PriceText { get; private set; }

        public int Cost => int.Parse(PriceText.text);

        private void OnValidate()
        {
            if (BuildPreviewSprite == null && Prefab != null)
                BuildPreviewSprite = Prefab.GetComponent<SpriteRenderer>().sprite;
        }
    }
}
