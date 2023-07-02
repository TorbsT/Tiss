using Assets.Scripts.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    public class TooltipSystem : MonoBehaviour
    {
        [System.Serializable]
        public class Tag
        {
            [field: SerializeField] public string Name { get; set; }
            [field: SerializeField] public Color Color { get; set; } = Color.magenta;
        }
        public static TooltipSystem Instance { get; private set; }

        [field: SerializeField] public Tooltip Tooltip { get; private set; }
        [field: SerializeField] public Canvas Canvas { get; private set; }
        [field: SerializeField] public List<Tag> Tags { get; private set; } = new();

        public void ShowTooltip(Tooltip.TooltipData data)
        {
            bool show = data != null;
            Tooltip.gameObject.SetActive(show);
            if (!show) return;
            Tooltip.Title.text = Interpret(data.Title);
            Tooltip.Desc.text = Interpret(data.Desc);
            Tooltip.Rarity.text = data.Rarity;
            Tooltip.Img.color = data.Color;
        }
        private void Awake()
        {
            Instance = this;
            Tooltip.gameObject.SetActive(false);
        }
        private void LateUpdate()
        {
            if (!Tooltip.gameObject.activeInHierarchy) return;
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Canvas.transform as RectTransform, Input.mousePosition, Canvas.worldCamera, out pos);
            Tooltip.transform.position = Canvas.transform.TransformPoint(pos);
        }
        private string Interpret(string text)
        {
            foreach (var tag in Tags)
            {
                text = text.Replace($"</{tag.Name}>", "</color>");

                string colorText = "#"+ColorUtility.ToHtmlStringRGB(tag.Color);
                text = text.Replace($"<{tag.Name}>", $"<color={colorText}>");
            }
            return text;
        }
    }
}