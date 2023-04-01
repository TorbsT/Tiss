using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundDescUI : MonoBehaviour
{
    public static RoundDescUI Instance { get; private set; }
    public string Text { get => text.text; set { text.text = value; } }
    public Color Color { get => text.color; set { text.color = value; } }

    [SerializeField] private TextMeshProUGUI text;
    private void Awake()
    {
        Instance = this;
    }
}
