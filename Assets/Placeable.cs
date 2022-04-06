using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeable : MonoBehaviour
{
    public bool Active { get => active; set { active = value; } }
    public bool Valid { get => valid; set { valid = value; } }
    public bool ParentToRoom { get => parentToRoom; set { parentToRoom = value; } }

    [Header("ASSIGN")]
    [SerializeField] private Color color = new(0xFF, 0xEE, 0x00, 0.25f);
    [SerializeField] private Color invalidColor = new(0xBF, 0x00, 0x47, 0.25f);
    [SerializeField] private bool parentToRoom = true;
    [SerializeField] private List<SpriteRenderer> renderers;

    [Header("DEBUG")]
    [SerializeField] private bool active;
    [SerializeField] private bool valid;

    private bool prevActive;
    private bool prevValid;
    private List<Color> defaultColors;

    private void Awake()
    {
        defaultColors = new();
        for (int i = 0; i < renderers.Count; i++)
        {
            defaultColors.Add(renderers[i].color);
        }
    }
    private void Update()
    {
        if (active != prevActive || valid != prevValid)
        {
            if (active)
            {
                if (valid)
                    SetPlaceColor();
                else
                    SetInvalidColor();
            } else
            {
                SetDefaultColors();
            }
            prevActive = active;
            prevValid = valid;
        }
    }
    private void SetDefaultColors()
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].color = defaultColors[i];
        }
    }
    private void SetPlaceColor()
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].color = color;
        }
    }
    private void SetInvalidColor()
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].color = invalidColor;
        }
    }
}
