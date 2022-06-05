using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public enum Direction
    {
        NORTH,
        EAST,
        SOUTH,
        WEST
    }
    public static ICollection<Direction> GetDirections()
    {
        HashSet<Direction> ds = new();
        ds.Add(Direction.NORTH);
        ds.Add(Direction.EAST);
        ds.Add(Direction.SOUTH);
        ds.Add(Direction.WEST);
        return ds;
    }
    public Vector2Int Id { get => id; set { id = value; RecalculateNeighbours(); } }
    public int NewPower { get => newPower; }
    public int Power { get => power; }
    public Direction NewDirection { get => newDirection; }
    public Transform MachineSpawn { get => machineSpawn; }
    public Transform SmallMachineSpawn { get => smallMachineSpawn; }
    public int NewRotation { get => newRotation; set { newRotation = value;
            if (newRotation % 4 == 0) newDirection = Direction.NORTH;
            else if (newRotation % 4 == 1) newDirection = Direction.EAST;
            else if (newRotation % 4 == 2) newDirection = Direction.SOUTH;
            else if (newRotation % 4 == 3) newDirection = Direction.WEST;
            RecalculateNeighbours();
        } }
    public int Rotation => rotation;
    public Border[] Borders => borders;
    
    [SerializeField] private bool debug;
    [SerializeField] private Vector2Int id;
    [SerializeField] private int newRotation;
    [SerializeField] private int newPower;
    [SerializeField] private Direction newDirection;
    [SerializeField, Range(0.01f, 10)] private float rotationAnimationDuration;
    [SerializeField, Range(0.01f, 10)] private float lightAnimationDuration;
    [SerializeField] private AnimationCurve rotationCurve;
    [SerializeField] private AnimationCurve lightCurve;
    [SerializeField] private Gradient lightGradient;
    [SerializeField] private SpriteRenderer[] renderers;
    [SerializeField] private Transform machineSpawn;
    [SerializeField] private Transform smallMachineSpawn;
    [SerializeField] private Border[] borders;

    private HashSet<Vector2Int> neighbours;
    private Coroutine rotanimRoutine;
    private Coroutine lightanimRoutine;
    private Dictionary<Generator, int> powerSources = new();
    private HashSet<IPowerListener> powerListeners = new();

    private int rotation;
    private int power;

    [Header("Rotation animation")]
    [SerializeField] private float rotanimStartRotation;
    [SerializeField] private float rotanimTargetRotation;
    [SerializeField] private float rotanimTime;
    [SerializeField] private bool rotanimRunning;

    [Header("Light animation")]
    [SerializeField] private float lightanimStartRatio;
    [SerializeField] private float lightanimTargetRatio;
    [SerializeField] private float lightanimTime;
    [SerializeField] private bool lightanimRunning;

    private bool newPowerOutdated;

    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (newPowerOutdated)
        {
            RecalculateNewPower();
            if (lightanimRunning)
            {
                UpdateTargetLightAnim();
            } else
            {
                lightanimRoutine = StartCoroutine(LightRoutine());
            }
        }
    }

    void OnEnable()
    {
        newPowerOutdated = true;
    }

    public void AddPowerSource(Generator powerSource, int power)
    {
        if (powerSources.ContainsKey(powerSource))
            Debug.LogWarning(this + "already has the powerSource " + powerSource);
        else
            powerSources.Add(powerSource, power);
        newPowerOutdated = true;
    }
    public void RemovePowerSource(Generator powerSource)
    {
        if (!powerSources.ContainsKey(powerSource)) return;
        powerSources.Remove(powerSource);
        newPowerOutdated = true;
    } 
    public void StartRotationAnimation()
    {
        if (rotanimRunning) StopAllCoroutines();
        rotanimRoutine = StartCoroutine(RotationRoutine());
    }

    public bool MoreDirectionThan(Room room, Direction direction)
    {
        Vector2Int loc = room.Id;
        if (direction == Direction.NORTH) return loc.y < id.y;
        if (direction == Direction.EAST) return loc.x < id.x;
        if (direction == Direction.SOUTH) return loc.y > id.y;
        else return loc.x > id.x;
    }

    public ICollection<Room> GetAccessibleRooms()
    {
        HashSet<Room> rooms = new();
        foreach (Vector2Int loc in neighbours)
        {
            Room r = SquareRoomSystem.Instance.LocToRoom(loc);
            if (r == null) continue;
            if (r.IsNeighbour(id)) rooms.Add(r);
        }
        return rooms;
    }
    public bool IsNeighbour(Vector2Int id)
    {
        foreach (Vector2Int i in neighbours)
        {
            if (i == id) return true;
        }
        return false;
    }

    public void AddPowerListener(IPowerListener listener)
    {
        powerListeners.Add(listener);
        listener.NewPower(newPower);
    }
    public void RemovePowerListener(IPowerListener listener)
    {
        powerListeners.Remove(listener);
    }
    private void RecalculateNewPower()
    {
        int max = 0;
        foreach (int power in powerSources.Values)
        {
            if (power > max) max = power;
        }
        newPower = max;
        newPowerOutdated = false;
        foreach (IPowerListener listener in powerListeners)
        {
            listener.NewPower(newPower);
        }
    }
    private void RecalculateNeighbours()
    {
        neighbours = new();
        if (newDirection != Direction.SOUTH) neighbours.Add(id + Vector2Int.up);
        if (newDirection != Direction.WEST) neighbours.Add(id + Vector2Int.right);
        if (newDirection != Direction.NORTH) neighbours.Add(id + Vector2Int.down);
        if (newDirection != Direction.EAST) neighbours.Add(id + Vector2Int.left);
    }
    private IEnumerator LightRoutine()
    {
        lightanimRunning = true;

        lightanimTime = 0f;

        UpdateTargetLightAnim();


        if (lightanimStartRatio != lightanimTargetRatio)
        {
            while (lightanimTime < lightAnimationDuration)
            {
                lightanimTime += Time.deltaTime;
                float animProgress = lightCurve.Evaluate(lightanimTime / lightAnimationDuration);
                float lightValue = animProgress * (lightanimTargetRatio - lightanimStartRatio) + lightanimStartRatio;
                foreach (SpriteRenderer renderer in renderers)
                {
                    renderer.color = lightGradient.Evaluate(lightValue);
                }
                
                yield return null;
            }
        }
        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.color = lightGradient.Evaluate(lightanimTargetRatio);
        }

        power = newPower;
        lightanimRunning = false;
    }
    private void UpdateTargetLightAnim()
    {
        float maxPower = GeneratorSystem.Instance.GlobalPower;
        lightanimStartRatio = Mathf.Clamp(power / maxPower, 0f, 1f);
        lightanimTargetRatio = Mathf.Clamp(newPower / maxPower, 0f, 1f);
    }
    private Direction RelativeToAbsoluteDirection(Direction d)
    {
        if (d == Direction.NORTH) return newDirection;
        if (newDirection == Direction.NORTH) return d;
        
        else if (newDirection == Direction.EAST)
        {
            if (d == Direction.EAST) return Direction.SOUTH;
            if (d == Direction.SOUTH) return Direction.WEST;
            if (d == Direction.WEST) return Direction.NORTH;
        }
        else if (newDirection == Direction.SOUTH)
        {
            if (d == Direction.EAST) return Direction.WEST;
            if (d == Direction.SOUTH) return Direction.NORTH;
            if (d == Direction.WEST) return Direction.EAST;
        }
        // if newDirection == west
        if (d == Direction.EAST) return Direction.NORTH;
        if (d == Direction.SOUTH) return Direction.EAST;
        if (d == Direction.WEST) return Direction.SOUTH;

        Debug.LogWarning("You fucking screwed up now");
        return Direction.NORTH;
    }
    private IEnumerator RotationRoutine()
    {
        rotanimRunning = true;

        /*
        foreach (Border border in borders)
        {
            border.gameObject.SetActive(false);
        }
        */

        int rotations = newRotation - rotation;
        rotanimTime = 0f;
        rotanimStartRotation = -90f * rotation;
        rotanimTargetRotation = -90f * newRotation;

        if (rotations != 0)
        {
            while (rotanimTime < rotationAnimationDuration)
            {
                rotanimTime += Time.deltaTime;
                float animProgress = rotationCurve.Evaluate(rotanimTime / rotationAnimationDuration);
                float rotationValue = rotanimStartRotation + animProgress * (rotanimTargetRotation - rotanimStartRotation);
                transform.rotation = Quaternion.Euler(0f, 0f, rotationValue);
                yield return null;
            }
        }
        transform.rotation = Quaternion.Euler(0f, 0f, rotanimTargetRotation);

        UpdateBorders();

        rotation = newRotation;
        rotanimRunning = false;
        RotationSystem.Instance.FinishedRotating(this);
    }
    public void AllOtherRoomsLoaded()
    {
        UpdateBorders();
    }
    private void UpdateBorders()
    {
        foreach (Border border in borders)
        {
            Direction absDirection = RelativeToAbsoluteDirection(border.RelativeDirection);
            Vector2Int loc = RelativeToAbsLoc(GetRelativeLoc(absDirection));
            Room r = GetRoom(loc);
            bool show = r == null;
            if (debug) Debug.Log(r.id + " " + loc + " " + absDirection + " " + border.RelativeDirection);
            border.gameObject.SetActive(show);
        }
    }
    private Room GetRoom(Vector2Int loc) => SquareRoomSystem.Instance.LocToRoom(loc);
    private Vector2Int RelativeToAbsLoc(Vector2Int loc) => id + loc;
    private Vector2Int GetRelativeLoc(Direction absDirection)
    {
        if (absDirection == Direction.NORTH) return Vector2Int.up;
        if (absDirection == Direction.EAST) return Vector2Int.right;
        if (absDirection == Direction.SOUTH) return Vector2Int.down;
        return Vector2Int.left;
    }

}
