using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tooltip : MonoBehaviour
{
    public Vector2 PositionToDisplay { get => positionToDisplay; set { positionToDisplay = value; } }
    public KeyCode KeyCode { get => keyCode; set { keyCode = value; keyCodeTextField.text = value.ToString(); } }

    [SerializeField] private KeyCode keyCode;
    [SerializeField] private TextMeshProUGUI keyCodeTextField;
    [SerializeField] private Vector2 positionToDisplay;
    private RectTransform rectTransform;
    private Image image;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    private void Update()
    {
        bool show = !ShopUI.Instance.isActiveAndEnabled;
        image.enabled = show;
        keyCodeTextField.enabled = show;
        rectTransform.anchoredPosition = UI.Instance.RectCalculator.WorldToScreenPoint(positionToDisplay);
    }
}
