using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pools;
using Pathfinding;

public class Generator : MonoBehaviour, IHPListener, IInventoryListener
{
    private class PathfindingStep
    {
        public Room room;
        public int range;

        public PathfindingStep(Room room, int range) {
            this.room = room;
            this.range = range;
        }

    }

    public Vector3 Position => transform.position;
    public bool NewRunning => newRunning;
    public int MaxPower { get => maxPower; set { maxPower = value; } }
    public float NewFuel => newFuel;
    public InventoryObject Inventory => inventory;
    public Item FuelItem => fuelItem;

    [SerializeField, Range(0f, 100f)] private float newFuel;
    [SerializeField, Range(0f, 100f)] private float fuel;
    [SerializeField] private Gradient logoGradient;
    [SerializeField] private SpriteRenderer logoRenderer;
    private int maxPower;
    [SerializeField] private int producedPower;
    [SerializeField] private Item fuelItem;
    [SerializeField, Range(0f, 100f)] private float chargePerFuel = 5f;
    [SerializeField] private int fpsPriority;
    [SerializeField] private float interactDelay;
    [SerializeField] private bool isSearchingRooms;
    [SerializeField] private int roomSearchFrames;
    [SerializeField] private bool updateRequested;
    [SerializeField] private bool newRunning;
    [SerializeField] private bool running;

    private float lastHoverTime;
    private Target target;
    private Tooltip tooltip;
    private Dictionary<Room, int> poweredRooms = new();
    private Dictionary<Room, int> newCalculatedRooms = new();
    private Coroutine pathfindingRoutine;
    private HashSet<IGeneratorListener> listeners = new();
    [SerializeField] private InventoryObject inventory;

    void Awake()
    {
        target = GetComponent<Target>();
        inventory = ScriptableObject.CreateInstance<InventoryObject>();
        inventory.SlotCount = 4;
        inventory.AddListener(this);
    }
    // Start is called before the first frame update
    void OnEnable()
    {
        maxPower = GeneratorSystem.Instance.GlobalPower;
        GeneratorSystem.Instance.Track(this);
        GetComponent<HP>().Set(100f);
    }
    void OnDisable()
    {
        GeneratorSystem.Instance.Untrack(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (chargePerFuel + newFuel <= 100f)
        {
            // Try consume
            if (InventoryExtensions.CanQuickRemove(inventory, fuelItem, 1))
            {
                InventoryExtensions.QuickRemove(inventory, fuelItem, 1);
                GetComponent<HP>().Increase(chargePerFuel);
            }
        }
        if (updateRequested || newRunning != running)
        {
            StartCalculatingPower();
            updateRequested = false;
            running = newRunning;
        }
    }

    public void AddListener(IGeneratorListener listener)
    {
        this.listeners.Add(listener);
        listener.StateChanged(this);
    }
    public void RemoveListener(IGeneratorListener listener)
    {
        listeners.Remove(listener);
    }
    public void RequestUpdate()
    {
        updateRequested = true;
    }
    private void StartCalculatingPower()
    {
        if (isSearchingRooms) StopAllCoroutines();
        if (GetComponent<RoomDweller>().Room == null) return;

        if (newRunning) producedPower = maxPower;
        else producedPower = 0;

        pathfindingRoutine = StartCoroutine(CalculateRoomsRoutine());
    }


    private IEnumerator CalculateRoomsRoutine()
    {
        isSearchingRooms = true;
        int frames = 1;
        Waiter waiter = new(1f / fpsPriority);
        Dictionary<Room, int> explored = new();
        Stack<PathfindingStep> roomsToExplore = new();
        roomsToExplore.Push(new (GetComponent<RoomDweller>().Room, producedPower));

        while (roomsToExplore.Count > 0)
        {
            PathfindingStep step = roomsToExplore.Pop();
            Room room = step.room;
            int range = step.range;
            if (explored.ContainsKey(room))
            {
                if (explored[room] >= range) continue;
                else explored.Remove(room);
            }
            explored.Add(room, range);

            if (range <= 0) continue;
            foreach (Room r in room.GetAccessibleRooms())
            {
                roomsToExplore.Push(new(r, range - 1));
            }
            if (waiter.CheckTime())
            {
                frames++;
                yield return null;
            }
        }
        newCalculatedRooms = explored;
        roomSearchFrames = frames;

        ApplyPowerToCalculatedRooms();  // ye

        isSearchingRooms = false;
    }
    private void ApplyPowerToCalculatedRooms()
    {
        if (poweredRooms != null)
        foreach (Room room in poweredRooms.Keys)
        {
            room.RemovePowerSource(this);
        }
        foreach (Room room in newCalculatedRooms.Keys)
        {
            room.AddPowerSource(this, newCalculatedRooms[room]);
        }
        Target.Discoverability discoverability = Target.Discoverability.hidden;
        if (newFuel > 0f) discoverability = Target.Discoverability.discoverable;
        target.SetDiscoverability(discoverability);
        poweredRooms = newCalculatedRooms;
        fuel = newFuel;
        Debug.Log("Æ");
        GeneratorSystem.Instance.NotifyPowerChanged();
    }
    public void NewHP(float oldHP, float newHP)
    {
        newFuel = Mathf.Clamp(newHP, 0f, 100f);
        logoRenderer.color = logoGradient.Evaluate(newFuel / 100f);
        newRunning = newFuel > 0f;
        if (newRunning) target.SetDiscoverability(Target.Discoverability.discoverable);
        else target.SetDiscoverability(Target.Discoverability.hidden);
        FireStateChanged();
    }
    private void FireStateChanged()
    {
        foreach (IGeneratorListener listener in listeners)
        {
            listener.StateChanged(this);
        }
    }

    public void StateChanged(InventoryObject obj)
    {
        FireStateChanged();
    }
}
public interface IGeneratorListener
{
    void StateChanged(Generator generator);
}
