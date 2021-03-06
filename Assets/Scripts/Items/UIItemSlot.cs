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
    public bool Locked => locked;

    [SerializeField] private bool debug;
    [SerializeField] private bool locked;
    [SerializeField] private Image background;
    [SerializeField] private Image lockedImg;
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
    public void SetLocked(bool locked)
    {
        this.locked = locked;
        Refresh();
    }
    public void Mark()
    {
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

        RectTransform rt = uiItem.RectTransform;
        rt.SetParent(rectTransform);
        rt.anchoredPosition = Vector2.zero;
        rt.anchorMin = Vector2.one*0f;
        rt.anchorMax = Vector2.one*1f;

        rt.sizeDelta = Vector2.one;
        if (lockedImg != null)
        {
            lockedImg.transform.SetAsLastSibling();
            lockedImg.gameObject.SetActive(locked);
            background.raycastTarget = !locked;
        }

        /*
        if (item != null && item.InventorySprite != null)
        {
            Vector2 spriteSize = item.InventorySprite.rect.size;
            float largest = Mathf.Max(spriteSize.x, spriteSize.y);
            spriteSize /= largest;
            rt.sizeDelta = spriteSize*0.9f;
        } else
        {
            rt.sizeDelta = Vector2.one*0.9f;
        }*/

        if (debug) Debug.Log("Refreshed " + this + ". the item is " + item + " with count " + quantity);
    }
    public void Click()
    {
        GetComponentInParent<IButtonReceiver>().Click(this, uiInventory);
    }
}
