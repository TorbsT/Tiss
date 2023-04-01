using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationSystem : MonoBehaviour, IEventListener
{
    public static RotationSystem Instance { get; private set; }

    private ICollection<Room> cachedRooms;
    private ICollection<Room> currentlyRotating;

    private void Awake()
    {
        Instance = this;
        EventSystem.AddEventListener(this, Event.NewRound);
        EventSystem.AddEventListener(this, Event.LoaderFinished);
    }
    public void RandomizeAll()
    {
        cachedRooms = GetComponent<LoaderSystem>().GetLoadedDictCopy().Values;
        currentlyRotating = new HashSet<Room>();
        foreach (Room room in cachedRooms)
        {
            room.NewRotation = Random.Range(0, 4);
            currentlyRotating.Add(room);
        }
        foreach (Room room in cachedRooms)
        {
            room.StartRotationAnimation();
        }
    }
    public void RotateAll()
    {
        cachedRooms = GetComponent<LoaderSystem>().GetLoadedDictCopy().Values;
        currentlyRotating = new HashSet<Room>();
        foreach (Room room in cachedRooms)
        {
            room.NewRotation += 1;
            currentlyRotating.Add(room);
        }
        foreach (Room room in cachedRooms)
        {
            room.StartRotationAnimation();
        }
    }
    public void FinishedRotating(Room room)
    {
        currentlyRotating.Remove(room);
        if (currentlyRotating.Count == 0)
        {
            // unsafe, state needed
            EventSystem.DeclareEvent(Event.RotationsDone);
        }
    }


    public void EventDeclared(Event e)
    {
        if (e == Event.LoaderFinished)
        {
            RandomizeAll();
        } else if (e == Event.NewRound)
        {
            // unsafe, might not be loaded
            RotateAll();
        }
    }
}
