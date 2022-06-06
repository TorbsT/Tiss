using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MachineSystem : MonoBehaviour, IEventListener
{
    [SerializeField] private GameObject generatorPrefab;
    [SerializeField] private GameObject shopPrefab;
    [SerializeField] private GameObject minerPrefab;
    [SerializeField] private GameObject teleporterPrefab;

    private Room startRoom;
    void Awake()
    {
        EventSystem.AddEventListener(this, Event.LoaderFinished);
    }
    public void EventDeclared(Event e)
    {
        if (e == Event.LoaderFinished)
        {
            startRoom = Player.Instance.GetComponent<RoomDweller>().Room;
            Vector2Int startRoomId = startRoom.Id;

            HashSet<Room> shopRooms = new();
            shopRooms.Add(SquareRoomSystem.Instance.LocToRoom(startRoomId + Vector2Int.left+Vector2Int.down));
            shopRooms.Add(SquareRoomSystem.Instance.LocToRoom(startRoomId + Vector2Int.left+Vector2Int.up));
            shopRooms.Add(SquareRoomSystem.Instance.LocToRoom(startRoomId + Vector2Int.right+Vector2Int.down));
            shopRooms.Add(SquareRoomSystem.Instance.LocToRoom(startRoomId + Vector2Int.right+Vector2Int.up));
            foreach (Room room in shopRooms)
            {
                SpawnMachine(shopPrefab, room);
            }

            HashSet<Room> teleporterRooms = new();
            teleporterRooms.Add(startRoom);
            Invoke(nameof(TempSpawnFarPortal), 5f);  // Holy shit
            foreach (Room room in teleporterRooms)
            {
                SpawnSmallMachine(teleporterPrefab, room);
            }

            SpawnMachine(generatorPrefab, startRoom);
            SpawnSmallMachine(minerPrefab, startRoom);

        }
    }
    private void TempSpawnFarPortal()
    {
        foreach (Node node in startRoom.GetComponent<AllToOne>().GetNodes())
        {
            if (node.StepsFromTarget == GeneratorSystem.Instance.GlobalPower - 1)
            {
                SpawnSmallMachine(teleporterPrefab, node.Room);
                break;
            }
        }
    }
    private void SpawnMachine(GameObject prefab, Room room)
    {
        if (room.GetComponentInChildren<Machine>() == null)
        {
            Transform t = EzPools.Instance.Depool(prefab).transform;
            t.position = room.MachineSpawn.position;
            t.localRotation = room.MachineSpawn.localRotation;
        }
    }
    private void SpawnSmallMachine(GameObject prefab, Room room)
    {
        Transform t = EzPools.Instance.Depool(prefab).transform;
        int smallMachinePlace = room.SmallMachines;
        t.position = room.SmallMachineSpawns[smallMachinePlace].position;
        t.localRotation = room.SmallMachineSpawns[smallMachinePlace].localRotation;
        room.SmallMachines++;
    }
}
