using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WelcomeScreen : MonoBehaviour, IEventListener
{
    [SerializeField, Range(0f, 5f)] private float minLoadingTime = 1f;
    [SerializeField, Range(0f, 3f)] private float fadeTime = 1f;

    private Image image;
    private ICollection<TextMeshProUGUI> textMeshProUGUIs;
    private float loadingTime = 0f;
    private bool hide;
    private bool fading;
    private bool done;
    private float alpha = 1f;

    private void Start()
    {
        EventSystem.AddEventListener(this, Event.RotationsDone);
        image = GetComponentInChildren<Image>();
        textMeshProUGUIs = GetComponentsInChildren<TextMeshProUGUI>();
    }
    void Update()
    {
        if (done) return;
        if (!fading)
        {
            loadingTime += Time.deltaTime;
            if (loadingTime >= minLoadingTime && hide)
            {
                fading = true;
            }
        } else
        {
            alpha -= Time.deltaTime / fadeTime;
            if (alpha <= 0f)
            {
                done = true;
                alpha = 0f;
                EventSystem.DeclareEvent(Event.ScreenVisible);
                gameObject.SetActive(false);
            }

            image.color = NewColor(image.color);
            foreach (var text in textMeshProUGUIs)
            {  // var suggested by this autocomplete thingy
                text.color = NewColor(text.color);
            }
        }

    }
    void IEventListener.EventDeclared(Event e)
    {
        if (e == Event.RotationsDone)
        {
            EventSystem.RemoveEventListener(this, Event.RotationsDone);
            hide = true;
        }
    }
    private Color NewColor(Color color) => new(color.r, color.g, color.b, alpha);
}
