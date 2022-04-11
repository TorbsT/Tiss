using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDweller : MonoBehaviour
{
    // Automatically identifies and parents to the respective room
    public Room Room => room;
    [SerializeField] private Room room;
    private HashSet<IRoomDwellerListener> listeners = new();

    private void Awake()
    {
        foreach (IRoomDwellerListener listener in GetComponents<IRoomDwellerListener>())
        {
            AddListener(listener);
        }
    }

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
            Transform parent = null;
            if (r != null) parent = r.transform;
            transform.parent = parent;
            foreach (IRoomDwellerListener listener in listeners)
            {
                listener.RoomChanged(room, r);
            }
            room = r;
        }
    }

    public void AddListener(IRoomDwellerListener listener)
    {
        listeners.Add(listener);
        listener.RoomChanged(room, room);
    }
}
