using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItem : MonoBehaviour
{
    public Item Item { get => item; set { item = value; } }
    public int Quantity { get => quantity; set { quantity = value; } }
    public RectTransform RectTransform => rectTransform;
    [SerializeField] private Item item;
    [SerializeField] private int quantity;

    [SerializeField] private Sprite emptySprite;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;

    private RectTransform rectTransform;
    private int oldQuantity = int.MinValue;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    private void OnEnable()
    {
        oldQuantity = int.MinValue;
    }
    private void Update()
    {
        if (item == null)
        {
            image.sprite = emptySprite;
            text.text = "";
            return;
        }
        if (image.sprite != item.InventorySprite)
        {
            image.sprite = item.InventorySprite;
        }
        if (quantity != oldQuantity)
        {
            string txt;
            if (quantity == 1) txt = "";
            else txt = quantity.ToString();
            text.text = txt;
            oldQuantity = quantity;
        }
    }

}
