using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tooltip : MonoBehaviour
{
    public Transform TransformToFollow { get => transformToFollow; set { transformToFollow = value; } }
    public KeyCode KeyCode { get => keyCode; set { keyCode = value; keyCodeTextField.text = value.ToString(); } }
    public Vector2 Offset { get => offset; set { offset = value; } }

    [SerializeField] private KeyCode keyCode;
    [SerializeField] private TextMeshProUGUI keyCodeTextField;
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
        //bool show = !ShopUI.Instance.isActiveAndEnabled && !MinerUI.Instance.isActiveAndEnabled && transformToFollow != null;
        //image.enabled = show;
        //keyCodeTextField.enabled = show;
        Vector2 posToFollow = transformToFollow.position;
        //if (show)
        rectTransform.anchoredPosition = UI.Instance.RectCalculator.WorldToScreenPoint(posToFollow+offset);
    }
}
