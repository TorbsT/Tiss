using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using EzPools;

public class Flash : MonoBehaviour
{
    public FlashObject FlashObject { get => flashObject; set { flashObject = value; } }
    private FlashObject flashObject;
    private float flashTime;
    private new Light2D light;

    private void Awake()
    {
        light = GetComponent<Light2D>();
    }
    private void OnEnable()
    {
        flashTime = 0f;
        light.enabled = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (flashObject == null) return;
        flashTime += Time.deltaTime;

        if (flashTime > flashObject.FlashDuration)
        {
            Manager.Instance.Enpool(gameObject);
        }

        float ratio = flashTime / flashObject.FlashDuration;
        light.color = flashObject.ColorGradient.Evaluate(ratio);
        light.intensity = flashObject.IntensityCurve.Evaluate(ratio)* flashObject.MaxIntensity;
    }
}
