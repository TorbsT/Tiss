using Assets.Scripts.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Systems
{
    internal class InfoSystem : MonoBehaviour
    {
        public static InfoSystem Instance { get; private set; }
        [field: SerializeField] public RectTransform InfoWrapper { get; private set; }

        public void Select(GameObject go)
        {
            if (go == null) Show(null);
            else if (go.name == "Infobutton")
            {
                Show(go.GetComponentInParent<ShopItem>());
            } else if (go.name == "Exitinfo")
            {
                Show(null);
            }
        }

        private void Awake()
        {
            Instance = this;
            Show(null);
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)
                && InfoWrapper.gameObject.activeInHierarchy)
            {
                Show(null);
            }
        }

        private void Show(ShopItem item)
        {
            bool showAtAll = item != null;
            string matchName = showAtAll ? item.Prefab.name : "none";

            InfoWrapper.gameObject.SetActive(showAtAll);
            RectTransform[] children = InfoWrapper.GetComponentsInChildren<RectTransform>(true);
            foreach (var childTransform in children)
            {
                GameObject child = childTransform.gameObject;
                if (childTransform.parent != InfoWrapper) continue;
                bool showThis = matchName == child.name;
                child.SetActive(showThis);
                if (showThis)
                {
                    Array.Find(child.GetComponentsInChildren<Image>(), match => match.gameObject.name == "Image")
                        .sprite = item.BuildPreviewSprite;
                }
            }
        }
    }
}
