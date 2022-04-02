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


    [SerializeField] private Image background;
    [SerializeField] private Color markColor;
    private Color unmarkColor;
    private RectTransform rectTransform;

    private UIItem uiItem;
    private UIInventory uiInventory;

    private Item item;
    private int quantity;

    private int index = int.MinValue;

    private void Awake()
    {
        uiInventory = GetComponentInParent<UIInventory>();
        rectTransform = GetComponent<RectTransform>();
        //unmarkColor = background.color;
    }
    public void SetItem(Item item)
    {
        this.item = item;
        Refresh();
    }
    public void SetQuantity(int quantity)
    {
        this.quantity = quantity;
        Refresh();
    }
    public void Mark()
    {
        Debug.Log("PENIS");
        //background.color = markColor;
    }
    public void Unmark()
    {
        //background.color = unmarkColor;
    }
    private void Refresh()
    {
        if (uiItem != null) uiItem.GetComponent<Destroyable>().Destroy();

        uiItem = EzPools.Instance.Depool(EzPools.Instance.UIItemPrefab).GetComponent<UIItem>();
        uiItem.Item = item;
        uiItem.Quantity = quantity;

        uiItem.RectTransform.SetParent(rectTransform);
        uiItem.RectTransform.anchoredPosition = Vector2.zero;
    }
    public void Click()
    {
        GetComponentInParent<IButtonReceiver>().Click(this, uiInventory);
    }
}
