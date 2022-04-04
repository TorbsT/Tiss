using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeable : MonoBehaviour
{
    public bool Active { get => active; set { active = value; } }
    public bool ParentToRoom { get => parentToRoom; set { parentToRoom = value; } }

    [SerializeField] private bool active;
    [SerializeField] private bool parentToRoom = true;
    [SerializeField] private Color color = new(0xFF, 0xEE, 0x00, 0.25f);
    [SerializeField] private List<SpriteRenderer> renderers;

    private bool prevActive;
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
        if (active != prevActive)
        {
            if (active)
            {
                for (int i = 0; i < renderers.Count; i++)
                {
                    renderers[i].color = color;
                }
            } else
            {
                for (int i = 0; i < renderers.Count; i++)
                {
                    renderers[i].color = defaultColors[i];
                }
            }
            prevActive = active;
        }
    }
}
