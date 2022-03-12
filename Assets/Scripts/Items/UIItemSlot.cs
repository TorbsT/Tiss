using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UIItemSlot : MonoBehaviour
{
    private UIInventoryManager manager => UIInventoryManager.Instance;
    public int Index { get { if (index < 0) index = int.Parse(gameObject.name); return index; } }
    public UIItem UIItem => uiItem;

    private RectTransform rectTransform;

    private UIItem uiItem;
    private UIInventory uiInventory;

    private int index = int.MinValue;

    private void Awake()
    {
        uiInventory = GetComponentInParent<UIInventory>();
        rectTransform = GetComponent<RectTransform>();
    }
    public void SetUIItem(UIItem item)
    {
        item.RectTransform.SetParent(rectTransform);
        item.RectTransform.anchoredPosition = Vector2.zero;
        uiItem = item;
    }
    public void Clicked()
    {
        manager.Clicked(this, uiInventory);
    }
}
