using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Teleporter : MonoBehaviour, IBatteryListener, IHPListener
{
    public int Id { get => id; set { id = value; } }
    public bool Powered => powered;

    [Header("ASSIGN")]
    [SerializeField] private Color inactiveColor;
    [SerializeField] private Color activeColor;
    [SerializeField] private SpriteRenderer rend;

    [Header("DEBUG")]
    [SerializeField] private bool powered;
    [SerializeField] private int id = -1;


    // Start is called before the first frame update
    void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        TeleporterSystem.Instance.Track(this);
    }
    void OnDisable()
    {
        TeleporterSystem.Instance.Untrack(this);
    }

    public void NewCharge(int oldCharge, int newCharge)
    {
        if (!isActiveAndEnabled) return;
        powered = newCharge > 0;
        if (powered)
        {
            rend.color = activeColor;
            GetComponent<Interactable>().Active = true;
            GetComponent<Target>().SetDiscoverability(Target.Discoverability.discoverable);
        } else
        {
            rend.color = inactiveColor;
            GetComponent<Interactable>().Active = false;
            GetComponent<Target>().SetDiscoverability(Target.Discoverability.hidden);
        }
    }

    public void NewHP(float oldHP, float newHP)
    {
        if (newHP <= 0f) GetComponent<Destroyable>().Destroy();
    }
}
