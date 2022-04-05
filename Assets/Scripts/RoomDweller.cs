using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDweller : MonoBehaviour
{
    // Automatically identifies and parents to the respective room
    public Room Room => room;
    [SerializeField] private Room room;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Room r = RoomManager.Instance.PosToRoom(transform.position);
        if (room != r)
        {
            transform.parent = r.transform;
            room = r;
        }
    }
}
