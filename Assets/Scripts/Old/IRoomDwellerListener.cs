using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRoomDwellerListener
{
    void RoomChanged(Room oldRoom, Room newRoom);
}
