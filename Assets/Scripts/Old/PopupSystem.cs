using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupSystem : MonoBehaviour
{
    public static PopupSystem Instance { get; private set; }
    public PopupRequest CurrentRequest => currentRequest;
    public static Interactor Interactor => InteractSystem.Interactor;

    [Header("ASSIGN")]
    [SerializeField] private GameObject pauseCockblock;
    [SerializeField] private GameObject deadCockblock;

    [Header("DEBUG")]
    [SerializeField] private bool debug;

    private PopupRequest currentRequest;
    private GameObject currentPopup;

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Pause()
    {
        PopupRequest r = new PopupRequest
        {
            closeRange = Mathf.Infinity,
            popup = pauseCockblock
        };
        Request(r);
    }

    public void Died()
    {
        PopupRequest r = new PopupRequest
        {
            closeRange = Mathf.Infinity,
            popup = deadCockblock
        };
        Request(r);
    }
    public void Request(PopupRequest request)
    {
        Debug.Log("Requested popup " + request.popup + " from " + request.interacted);
        currentRequest = request;
        Refresh();
    }
    public void Hide()
    {
        Debug.Log("Requested Hide ");
        currentRequest = null;
        Refresh();
    }
    private void Refresh()
    {
        if (currentPopup != null)
        {
            currentPopup.SetActive(false);
            currentPopup = null;
        }

        if (currentRequest != null)
        {
            currentPopup = currentRequest.popup;
            if (currentPopup == null)
            {
                Debug.LogError("Request from " + currentRequest.interacted + "has null popup");
            }
            currentPopup.SetActive(true);
        }
    }
}
public class PopupRequest
{
    public GameObject popup;
    public Interactable interacted;
    public float closeRange;
}
