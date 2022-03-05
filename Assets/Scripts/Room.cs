using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Room : MonoBehaviour
{
    public enum Direction
    {
        NORTH,
        EAST,
        SOUTH,
        WEST
    }
    public Vector2Int Id { get => id; set { id = value; RecalculateNeighbours(); } }
    public int NewPower { get => newPower; }
    public int Power { get => power; }
    public Direction NewDirection { get => newDirection; }
    public int NewRotation { get => newRotation; set { newRotation = value;
            if (newRotation % 4 == 0) newDirection = Direction.NORTH;
            else if (newRotation % 4 == 1) newDirection = Direction.EAST;
            else if (newRotation % 4 == 2) newDirection = Direction.SOUTH;
            else if (newRotation % 4 == 3) newDirection = Direction.WEST;
            RecalculateNeighbours();
        } }
    public int Rotation => rotation;
    
    [SerializeField] private Vector2Int id;
    [SerializeField] private int newRotation;
    [SerializeField] private int newPower;
    [SerializeField] private Direction newDirection;
    [SerializeField, Range(0.01f, 10)] private float rotationAnimationDuration;
    [SerializeField, Range(0.01f, 10)] private float lightAnimationDuration;
    [SerializeField] private AnimationCurve rotationCurve;
    [SerializeField] private AnimationCurve lightCurve;
    [SerializeField] private Gradient lightGradient;
    [SerializeField] private SpriteRenderer floorSprite;

    private HashSet<Vector2Int> neighbours;
    private Coroutine rotanimRoutine;
    private Coroutine lightanimRoutine;
    private Dictionary<Generator, int> powerSources = new();

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
        if (rotanimRunning) return;
        rotanimRoutine = StartCoroutine(RotationRoutine());
    }
    public ICollection<Room> GetAccessibleRooms()
    {
        HashSet<Room> rooms = new();
        foreach (Vector2Int loc in neighbours)
        {
            Room r = RoomManager.Instance.LocToRoom(loc);
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


    private void RecalculateNewPower()
    {
        int max = 0;
        foreach (int power in powerSources.Values)
        {
            if (power > max) max = power;
        }
        newPower = max;
        newPowerOutdated = false;
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
                floorSprite.color = lightGradient.Evaluate(lightValue);
                yield return null;
            }
        }
        floorSprite.color = lightGradient.Evaluate(lightanimTargetRatio);

        power = newPower;
        lightanimRunning = false;
    }
    private void UpdateTargetLightAnim()
    {
        float maxPower = RoomManager.Instance.GeneratorPower;
        lightanimStartRatio = Mathf.Clamp(power / maxPower, 0f, 1f);
        lightanimTargetRatio = Mathf.Clamp(newPower / maxPower, 0f, 1f);
    }
    private IEnumerator RotationRoutine()
    {
        rotanimRunning = true;

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

        rotation = newRotation;
        rotanimRunning = false;
    }

}
