using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tooltip : MonoBehaviour
{
    public Vector2 PositionToDisplay { get => positionToDisplay; set { positionToDisplay = value; } }
    public KeyCode KeyCode { get => keyCode; set { keyCode = value; keyCodeTextField.text = value.ToString(); } }

    [SerializeField] private KeyCode keyCode;
    [SerializeField] private TextMeshProUGUI keyCodeTextField;
    [SerializeField] private Vector2 positionToDisplay;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        rectTransform.anchoredPosition = UI.Instance.RectCalculator.WorldToScreenPoint(positionToDisplay);
    }
}
