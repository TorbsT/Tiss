using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RoundTimer : MonoBehaviour
{
    public float Time { get => float.Parse(textField.text); set { textField.text = value.ToString(); } }
    public float LoadProgress { get => image.fillAmount; set { image.fillAmount = value; } }
    public Color TextColor { get => textField.color; set { textField.color = value; } }
    public Color ImageColor { get => image.color; set { image.color = value; } }
    public static RoundTimer Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private Image image;
    private void Awake()
    {
        Instance = this;
    }
}
