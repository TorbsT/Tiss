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
    public class Tooltip : MonoBehaviour
    {
        public class TooltipData
        {
            public TooltipData(string title, string desc, Color color, string rarity)
            {
                Title = title;
                Desc = desc;
                Color = color;
                Rarity = rarity;
            }
            public string Title { get; set; }
            public string Desc { get; set; }
            public string Rarity { get; set; }
            public Color Color { get; set; }
        }
        [field: SerializeField] public RectTransform Transform { get; private set; }
        [field: SerializeField] public TextMeshProUGUI Title { get; private set; }
        [field: SerializeField] public TextMeshProUGUI Desc { get; private set; }
        [field: SerializeField] public TextMeshProUGUI Rarity { get; private set; }
        [field: SerializeField] public Image Img { get; private set; }

        private void OnValidate()
        {
            if (Transform == null)
                Transform = GetComponent<RectTransform>();
        }
    }
}
