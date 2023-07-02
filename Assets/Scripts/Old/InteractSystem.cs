using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSystem : MonoBehaviour
{
    public static Interactor Interactor => Instance.interactor;
    public static InteractSystem Instance { get; private set; }

    [SerializeField] private RectTransform tooltipWrapper;
    private Tooltip tooltip;

    private Interactor interactor;
    private Interactable hovering;
    private Interactable prevHovering;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        interactor = Player.Instance.GetComponent<Interactor>();
    }
    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {
        hovering = null;
        prevHovering = null;
        if (tooltip != null) Pools.TooltipPool.Instance.Enpool(tooltip);
        tooltip = null;
    }
    // Update is called once per frame
    void Update()
    {
        hovering = null;
        float leastMouseDistance = float.MaxValue;
        Vector2 interactorPos = interactor.transform.position;
        Vector2 mousePos = UI.Instance.RectCalculator.ScreenPointToWorld(Input.mousePosition);
        foreach (Interactable interactable in FindObjectsOfType<Interactable>())
        {
            if (!interactable.Active || !interactable.isActiveAndEnabled) continue;
            Vector2 p = interactable.transform.position;
            if ((p-interactorPos).sqrMagnitude <= Mathf.Pow(interactable.OpenRange,2))
            {
                // Valid, but is it shortest from the mouse?
                float mouseDistance = (p - mousePos).sqrMagnitude;
                if (mouseDistance < leastMouseDistance)
                {
                    hovering = interactable;
                    leastMouseDistance = mouseDistance;
                }
            }
        }

        if (prevHovering != hovering)
        {
            if (tooltip != null)
            {
                Pools.TooltipPool.Instance.Enpool(tooltip);
                tooltip = null;
            }
            
            if (hovering != null)
            {
                Tooltip tt = Pools.TooltipPool.Instance.Depool();
                tt.KeyCode = KeyCode.F;
                tt.transform.SetParent(tooltipWrapper, false);
                tt.TransformToFollow = hovering.transform;
                tt.Offset = hovering.Offset;
                tooltip = tt;
            }
        }

        prevHovering = hovering;
    }

    internal bool CanInteract()
    {
        return hovering != null;
    }

    internal void Hide()
    {
        enabled = false;
    }

    internal PopupRequest Interact()
    {
        Teleporter teleporter = hovering.GetComponent<Teleporter>();
        if (teleporter != null)
        {
            TeleporterSystem.Instance.Teleport(interactor.transform, teleporter);
            return null;
        } else
        {
            PopupRequest r = new PopupRequest
            {
                closeRange = hovering.CloseRange,
                interacted = hovering
            };
            if (hovering.GetComponent<Generator>() != null) r.popup = GeneratorUI.Instance.gameObject;
            else if (hovering.GetComponent<Turret>() != null) r.popup = TurretUI.Instance.gameObject;
            else if (hovering.GetComponent<Miner>() != null) r.popup = MinerUI.Instance.gameObject;
            else if (hovering.GetComponent<Shop>() != null) r.popup = ShopUI.Instance.gameObject;
            return r;
        }
    }

    internal void Show()
    {
        enabled = true;
    }
}
