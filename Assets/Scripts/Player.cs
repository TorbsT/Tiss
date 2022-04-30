using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    [SerializeField] private bool gizmos;
    [SerializeField] private Item clubItem;
    private Locomotion locomotion;
    private Target target;
    private Vector2 lookingAt;
    private HP hp;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        locomotion = GetComponent<Locomotion>();
        target = GetComponent<Target>();
        hp = GetComponent<HP>();
    }

    private void Start()
    {
        target.SetDiscoverability(Target.Discoverability.discoverable);
        hp.Set(100f);
        GetComponent<IWalletProvider>().Wallet.Shitcoin = 150;
        if (!InventoryExtensions.CanQuickRemove(GetComponent<PlayerInventoryAPI>().Main, clubItem, 1))
        {
            InventoryExtensions.QuickAdd(GetComponent<PlayerInventoryAPI>().Main, clubItem, 1);
        }
    }
    private void OnDrawGizmos()
    {
        if (!gizmos) return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(lookingAt, 0.5f);
        Gizmos.DrawLine(transform.position, lookingAt);
    }

    // Update is called once per frame
    void Update()
    {
        float ws = Input.GetAxisRaw("Vertical");
        float ad = Input.GetAxisRaw("Horizontal");
        Vector2 mouseScreenPosition = Input.mousePosition;
        Vector2 mousePos = UI.Instance.RectCalculator.ScreenPointToWorld(mouseScreenPosition);
        GetComponent<LookAt>().LookingAt = mousePos;



        // movement
        Vector2 direction = new(ad, ws);
        locomotion.Direction = direction;
    }
}
