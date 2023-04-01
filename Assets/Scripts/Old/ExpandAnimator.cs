using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandAnimator : MonoBehaviour, IHPListener
{
    [SerializeField, Range(0f, 1f)] private float expandAmount = 0.1f;
    [SerializeField, Range(0.01f, 1f)] private float duration = 0.5f;
    [SerializeField] private AnimationCurveObject curve;
    [SerializeField] private bool playOnDamage = true;
    [SerializeField] private bool playOnHeal = false;
    [SerializeField] private bool playOnNoChange = false;

    private float time;
    private float current = 1f;
    public void NewHP(float oldHP, float newHP)
    {
        bool play = newHP < oldHP && playOnDamage || newHP > oldHP && playOnHeal || newHP == oldHP && playOnNoChange;
        if (play)
        {
            StopAllCoroutines();
            if (isActiveAndEnabled)
            StartCoroutine(Animation());
        }
    }
    private IEnumerator Animation()
    {
        time = 0f;
        SetScale(1f);

        while (time <= 1f)
        {
            yield return null;
            float newScale = 1f + curve.Curve.Evaluate(time) * expandAmount;
            SetScale(newScale);
            time += Time.deltaTime / duration;
        }
        SetScale(1f);
    }
    private void SetScale(float newScale)
    {
        float amountToMultiply = newScale / current;
        if (amountToMultiply == Mathf.Infinity)
        {
            Debug.Log(newScale + " " + current);
        }
        transform.localScale *= amountToMultiply;
        current = newScale;
    }
}
