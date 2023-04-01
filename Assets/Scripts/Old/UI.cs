using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour, IEventListener
{
    public enum State
    {
        Free,
        Popup
    }
    public RectCalculator RectCalculator => rectCalculator;

    public RectTransform HealthbarTransform => healthbarTransform;
    public static UI Instance { get; private set; }


    [SerializeField] private GameObject COck;

    [SerializeField] private RectTransform healthbarTransform;
    [SerializeField] private WelcomeScreen welcomeScreen;
    private RectCalculator rectCalculator;
    private State state;
    private bool dead;
    private bool active;
    private void Awake()
    {
        Instance = this;
        rectCalculator = GetComponent<RectCalculator>();
        welcomeScreen.gameObject.SetActive(true);
    }
    private void Start()
    {
        EventSystem.AddEventListener(this, Event.ScreenVisible);
    }
    private void Update()
    {
        if (!active || dead) return;
        if (state == State.Free)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                state = State.Popup;
                PopupSystem.Instance.Pause();
                InteractSystem.Instance.Hide();
            }
            else if (Input.GetKeyDown(KeyCode.F) && InteractSystem.Instance.CanInteract())
            {
                PopupRequest request = InteractSystem.Instance.Interact();
                if (request != null)
                {
                    state = State.Popup;
                    PopupSystem.Instance.Request(request);
                    InteractSystem.Instance.Hide();
                }
            }
        } else if (state == State.Popup)
        {
            bool exit = Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.F);
            PopupRequest request = PopupSystem.Instance.CurrentRequest;
            Interactable interacted = request.interacted;
            Interactor interactor = InteractSystem.Interactor;
            Vector2 playerPos = interactor.transform.position;
            
            if (interacted != null)
            {
                Vector2 interactedPos = interacted.transform.position;
                exit = exit || !interacted.Active || !interacted.isActiveAndEnabled;
                exit = exit || (playerPos - interactedPos).magnitude > interacted.CloseRange;
            }

            if (exit)
            {
                state = State.Free;
                PopupSystem.Instance.Hide();
                InteractSystem.Instance.Show();
            }
            
        }
        /*
        if (!dead && Input.GetKeyDown(KeyCode.Escape))
        {
            bool startPause = !pauseMenu.activeSelf;
            pauseMenu.SetActive(startPause);
            if (startPause)
            {
                PauseSystem.Instance.Pause();
            } else
            {
                PauseSystem.Instance.Resume();
            }
        }
        */
    }
    public void Died()
    {
        dead = true;
        state = State.Popup;
        PopupSystem.Instance.Died();
    }
    public void Restart()
    {
        SceneManager.LoadSceneAsync("Game");
    }
    public void MainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
    void IEventListener.EventDeclared(Event e)
    {
        if (e == Event.ScreenVisible)
        {
            active = true;
        }
    }
}
