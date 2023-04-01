using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pools;

public class HealthbarDisplayer : MonoBehaviour, IHPListener
{
    [Header("CONFIG")]
    [SerializeField] private bool alwaysDisplay;
    [SerializeField, Range(0f, 5f)] private float changeDisplayTime = 2f;
    [SerializeField] private Vector2 offset = Vector2.up;
    [SerializeField] private GradientObject gradientObject;

    [Header("DEBUG")]
    [SerializeField] private bool displaying;
    private float hp;
    private float minHP = 0f;
    private float maxHP = 100f;
    private float timeSinceChange;
    private Healthbar healthbar;

    void OnDisable()
    {
        if (healthbar != null) Destroy();
    }
    void OnEnable()
    {
        Invoke(nameof(TryCreate), 1f);
    }
    void Update()
    {
        timeSinceChange += Time.deltaTime;
        displaying = alwaysDisplay || timeSinceChange <= changeDisplayTime;
        if (healthbar != null)
        if (healthbar.Display != displaying)
            healthbar.Display = displaying;
    }

    public void NewHP(float oldHP, float newHP)
    {
        hp = newHP;
        Refresh();
    }
    private void TryCreate()
    {
        healthbar = Create();
        healthbar.transform.SetParent(UI.Instance.HealthbarTransform);
        healthbar.Offset = offset;
        healthbar.TransformToFollow = transform;
        Refresh();
    }
    private void Refresh()
    {
        if (healthbar != null)
        {
            float ratio = (hp - minHP) / (maxHP - minHP);
            healthbar.Amount = ratio;
            healthbar.Color = gradientObject.Gradient.Evaluate(ratio);
            timeSinceChange = 0f;
        }
    }
    private Healthbar Create()
    {
        return HealthbarPool.Instance.Depool();
    }
    private void Destroy()
    {
        HealthbarPool.Instance.Enpool(healthbar);
        healthbar = null;
    }

}
