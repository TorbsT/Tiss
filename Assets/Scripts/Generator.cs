using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pools;

public class Generator : MonoBehaviour, IInteractable
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
    public bool Running => fuel > 0f;
    public int MaxPower { get => maxPower; set { maxPower = value; } }

    [SerializeField, Range(0f, 100f)] private float fuel;
    [SerializeField] private Room parentRoom;
    [SerializeField] private int maxPower;
    [SerializeField] private int producedPower;
    [SerializeField] private int fpsPriority;
    [SerializeField] private float interactDelay;
    [SerializeField] private bool isSearchingRooms;
    [SerializeField] private int roomSearchFrames;

    private float lastHoverTime;
    private Tooltip tooltip;
    private Dictionary<Room, int> poweredRooms = new();
    private Dictionary<Room, int> newCalculatedRooms = new();
    private Coroutine pathfindingRoutine;

    // Start is called before the first frame update
    void OnEnable()
    {
        fuel = 100f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartApplyingPower()
    {
        if (parentRoom == null) return;

        if (Running) producedPower = maxPower;
        else producedPower = 0;

        pathfindingRoutine = StartCoroutine(PowerRoomsRoutine());
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
    private IEnumerator PowerRoomsRoutine()
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
        isSearchingRooms = false;
        roomSearchFrames = frames;
        ApplyPowerToCalculatedRooms();
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

        poweredRooms = newCalculatedRooms;
        fuel = Mathf.Max(0f, fuel - 25f);
    }


    // IInteractable
    public bool CanHover()
    {
        return true;
        return fuel <= 75f && lastHoverTime + interactDelay > Time.time;
    }

    public bool CanInteract()
    {
        return true;
    }

    public void Hover()
    {
        tooltip = TooltipPool.Instance.Depool();
        tooltip.KeyCode = KeyCode.F;
        tooltip.transform.SetParent(UI.Instance.transform, false);
        tooltip.PositionToDisplay = transform.position;
    }

    public void Unhover()
    {
        TooltipPool.Instance.Enpool(tooltip);
        tooltip = null;
        lastHoverTime = Time.time;
    }

    public void Interact()
    {
        fuel = Mathf.Min(fuel + 25f, 100f);
    }
}
