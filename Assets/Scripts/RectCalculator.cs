using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectCalculator : MonoBehaviour
{
    private RectTransform rectTransform;
    private new Camera camera;
    [SerializeField] private bool gizmos;
    [SerializeField] private Vector2 worldPosition;
    [SerializeField] private Vector2 viewportPosition;
    [SerializeField] private Vector2 anchoredPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        camera = FindObjectOfType<Camera>();
    }
    private void OnDrawGizmosSelected()
    {
        if (gizmos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(worldPosition, Vector2.one * 50f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(viewportPosition, Vector2.one * 50f);
            Gizmos.color = Color.green;
            Gizmos.DrawCube(anchoredPosition, Vector2.one * 50f);
        }
    }
    public Vector2 WorldToScreenPoint(Vector2 worldPosition)
    {
        Vector2 view = camera.WorldToScreenPoint(worldPosition);
        view -= rectTransform.sizeDelta / 2f;
        this.viewportPosition = view;
        return view;
    }
}
