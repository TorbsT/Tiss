using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pools;
using Pathfinding;

public class Generator : MonoBehaviour, IAttackable, IInteractableListener
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
    public bool Running => running;
    public int MaxPower { get => maxPower; set { maxPower = value; } }

    [SerializeField, Range(0f, 100f)] private float newFuel;
    [SerializeField, Range(0f, 100f)] private float fuel;
    [SerializeField] private Room parentRoom;
    [SerializeField] private int maxPower;
    [SerializeField] private int producedPower;
    [SerializeField] private int fpsPriority;
    [SerializeField] private float interactDelay;
    [SerializeField] private bool isSearchingRooms;
    [SerializeField] private int roomSearchFrames;
    [SerializeField] private bool updateRequested;
    [SerializeField] private bool running;

    private float lastHoverTime;
    private Target target;
    private Tooltip tooltip;
    private Dictionary<Room, int> poweredRooms = new();
    private Dictionary<Room, int> newCalculatedRooms = new();
    private Coroutine pathfindingRoutine;

    void Awake()
    {
        target = GetComponent<Target>();
    }
    void Start()
    {
        GetComponent<Interactable>().AddListener(this);
    }
    // Start is called before the first frame update
    void OnEnable()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (updateRequested || newFuel != fuel)
        {
            if (isSearchingRooms) StopAllCoroutines();
            StartCalculatingPower();
            updateRequested = false;
        }
    }
    public void RequestUpdate()
    {
        updateRequested = true;
    }
    public void Place(Room room)
    {
        if (room == null) Debug.LogWarning("Tried placing a generator in null room");
        this.parentRoom = room;
        if (room != null)
        {
            transform.SetParent(room.transform);
            transform.localPosition = Vector2.zero;
        }
    }
    private void StartCalculatingPower()
    {
        if (isSearchingRooms) StopCoroutine(pathfindingRoutine);
        if (parentRoom == null) return;

        if (running) producedPower = maxPower;
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
        roomsToExplore.Push(new (parentRoom, producedPower));

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
    }

    public void Interact(Interactor interactor)
    {
        AddFuel(25f);
    }
    public void Attack(float damage)
    {
        AddFuel(-damage);
    }
    public void AddFuel(float amount)
    {
        SetFuel(newFuel + amount);
    }
    private void SetFuel(float amount)
    {
        newFuel = Mathf.Clamp(amount, 0f, 100f);
        running = newFuel > 0f;
        if (running) target.SetDiscoverability(Target.Discoverability.discoverable);
        else target.SetDiscoverability(Target.Discoverability.hidden);
    }
}
