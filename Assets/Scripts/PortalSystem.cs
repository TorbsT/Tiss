using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class PortalSystem : MonoBehaviour
{
    public static PortalSystem Instance { get; private set; }
    public int ZombiesToSpawn { get => zombiesToSpawn; set { zombiesToSpawn = value; } }
    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private int minRoomsAway = 1;
    [SerializeField] private int zombiesToSpawn;
    private HashSet<Portal> portals;
    private HashSet<Node> chosenNodes;
    private Coroutine portalRoutine;
    private bool portalRoutineRunning;

    public void StartSpawningPortals()
    {
        StopPortalRoutine();
        portalRoutine = StartCoroutine(PortalRoutine());
    }
    public void StartSpawningZombies()
    {
        StopPortalRoutine();
        foreach (Portal portal in portals)
        {
            portal.StartSpawning();
        } 
    }
    public void Add(Portal portal)
    {
        portals.Add(portal);
    }
    public void Remove(Portal portal)
    {
        portals.Remove(portal);
    }

    private void StopPortalRoutine()
    {
        if (portalRoutineRunning)
        {
            StopCoroutine(portalRoutine);
        }
    }
    private IEnumerator PortalRoutine()
    {
        portalRoutineRunning = true;

        if (portals != null)
        {
            HashSet<Portal> ps = new();
            foreach (Portal portal in portals)
            {
                ps.Add(portal);
            }
            foreach (Portal portal in ps)
            {
                portal.GetComponent<Destroyable>().Destroy();
            }
        }

        portals = new();

        bool canStart = true;
        Room startRoom = Player.Instance.GetComponent<RoomDweller>().Room;
        if (startRoom == null)
        {
            Debug.LogWarning("Can't spawn portals, target is out of bounds");
            canStart = false;
        }

        AllToOne ato = null;
        if (canStart)
        {
            ato = startRoom.GetComponent<AllToOne>();
            if (ato == null)
            {
                Debug.LogWarning("Can't spawn portals, AllToOne is null");
                canStart = false;
            }
        }

        if (canStart)
        {
            if (ato.CurrentState != AllToOne.State.calculated)
            {
                Debug.LogWarning("Can't spawn portals, AllToOne's state is " + ato.CurrentState);
                canStart = false;
            }
        }

        if (canStart)
        {
            chosenNodes = new();
            foreach (Room.Direction direction in Room.GetDirections())
            {
                Node n = ExtremeNode(direction);
                if (n != null) chosenNodes.Add(n);
                yield return null;
            }

            int totalSteps = 0;
            foreach (Node node in chosenNodes)
            {
                totalSteps += node.StepsFromTarget;
            }
            int zombiesLeftToDelegate = zombiesToSpawn;
            foreach (Node node in chosenNodes)
            {
                Portal portal = EzPools.Instance.Depool(portalPrefab).GetComponent<Portal>();
                portal.transform.position = node.Room.transform.position;
                int delegatedZombies = Mathf.FloorToInt((float)node.StepsFromTarget/totalSteps*zombiesLeftToDelegate);
                Debug.Log(totalSteps + " " + delegatedZombies + " " + node.StepsFromTarget + " " + zombiesLeftToDelegate);
                portal.ZombiesToSpawn = delegatedZombies;
                zombiesLeftToDelegate -= delegatedZombies;
                portals.Add(portal);
            }
        }

        portalRoutineRunning = false;

        Node ExtremeNode(Room.Direction direction)
        {
            Node best = null;

            foreach (Node node in ato.GetNodes())
            {
                if (node.StepsFromTarget < minRoomsAway) continue;
                if (best == null)
                {
                    best = node;
                    continue;
                }
                
                if (node.Room.MoreDirectionThan(best.Room, direction))
                {
                    best = node;
                }
            }
            return best;
        }
    }
}
