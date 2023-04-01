using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Transform TransformToFollow { get => transformToFollow; set { transformToFollow = value; } }
    public Vector2 Offset { get => offset; set { offset = value; } }
    public float Amount { get => fillAmount; set { fillAmount = value; } }
    public Color Color { get => color; set { color = value; } }
    public bool Display { get => display; set { display = value; } }

    [SerializeField] private float fillAmount;
    [SerializeField] private bool display;
    [SerializeField] private Color color;
    [SerializeField] private Image fillImg;
    private Vector2 offset;
    private RectTransform rectTransform;
    private Transform transformToFollow;
    private Image image;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    private void Update()
    {
        bool show = display && !ShopUI.Instance.isActiveAndEnabled && !MinerUI.Instance.isActiveAndEnabled && transformToFollow != null;
        image.enabled = show;
        fillImg.enabled = show;
        fillImg.fillAmount = fillAmount;
        fillImg.color = color;
        Vector2 posToFollow = transformToFollow.position;
        if (show)
            rectTransform.anchoredPosition = UI.Instance.RectCalculator.WorldToScreenPoint(posToFollow + offset);
    }
}
