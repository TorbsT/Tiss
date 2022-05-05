using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineSystem : MonoBehaviour, IEventListener
{
    [SerializeField] private GameObject generatorPrefab;
    [SerializeField] private GameObject shopPrefab;
    [SerializeField] private GameObject minerPrefab;
    void Awake()
    {
        EventSystem.AddEventListener(this, Event.LoaderFinished);
    }
    public void EventDeclared(Event e)
    {
        if (e == Event.LoaderFinished)
        {
            Room playerRoom = Player.Instance.GetComponent<RoomDweller>().Room;
            Vector2Int playerRoomId = playerRoom.Id;
            HashSet<Room> shopRooms = new();
            shopRooms.Add(SquareRoomSystem.Instance.LocToRoom(playerRoomId + Vector2Int.left+Vector2Int.down));
            shopRooms.Add(SquareRoomSystem.Instance.LocToRoom(playerRoomId + Vector2Int.left+Vector2Int.up));
            shopRooms.Add(SquareRoomSystem.Instance.LocToRoom(playerRoomId + Vector2Int.right+Vector2Int.down));
            shopRooms.Add(SquareRoomSystem.Instance.LocToRoom(playerRoomId + Vector2Int.right+Vector2Int.up));

            SpawnMachine(generatorPrefab, playerRoom);
            foreach (Room room in shopRooms)
            {
                SpawnMachine(shopPrefab, room);
            }
            SpawnSmallMachine(minerPrefab, playerRoom);
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
        t.position = room.SmallMachineSpawn.position;
        t.localRotation = room.SmallMachineSpawn.localRotation;
    }
}
