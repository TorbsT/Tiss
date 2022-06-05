using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystem : MonoBehaviour
{
    // Intended as replacement for SystemListeners.
    // Since Systems use the singleton pattern they can use this

    [SerializeField] private bool debugEvents;
    private static bool sDebugEvents;
    private static Dictionary<Event, HashSet<IEventListener>> eventListeners = null;

    private void OnEnable()
    {
        EnsureDict();
        sDebugEvents = debugEvents;
    }
    private void OnDisable()
    {
        eventListeners = null;
    }
    /**
     * Listens to all events.
     */
    /* please do not use
    public static void AddListener(IEventListener listener)
    {
        EnsureDict();
        foreach (Event e in eventListeners.Keys)
        {
            eventListeners[e].Add(listener);
        }
    }
    /**
     * Listen to a single event.
     */
    public static void AddEventListener(IEventListener listener, Event e)
    {
        EnsureDict();
        if (!eventListeners.ContainsKey(e)) eventListeners.Add(e, new());
        eventListeners[e].Add(listener);
    }

    /**
     * Remove listening to a single event.
     */
    public static void RemoveEventListener(IEventListener listener, Event e)
    {
        EnsureDict();
        eventListeners[e].Remove(listener);
    }
    /**
     * Remove listening to all events.
     */
    /* please do not use
    public static void RemoveListener(IEventListener listener)
    {
        EnsureDict();
        foreach (Event e in eventListeners.Keys)
        {
            RemoveEventListener(listener, e);
        }
    }
    */
    /**
     * Every object listening to this event,
     * will be notified
     */
    public static void DeclareEvent(Event e)
    {
        EnsureDict();
        if (sDebugEvents)
        {
            Debug.Log(e);
        }
        HashSet<IEventListener> ls = new();
        foreach (IEventListener listener in eventListeners[e])
        {
            ls.Add(listener);
        }
        foreach (IEventListener listener in ls)
        {
            listener.EventDeclared(e);
        }
        if (sDebugEvents) Debug.Log(ls.Count);
    }

    private static void EnsureDict()
    {
        if (eventListeners == null)
        {
            eventListeners = new();
            foreach (Event e in Enum.GetValues(typeof(Event)))
            {
                eventListeners.Add(e, new());
            }
        }
    }
}
public enum Event
{
    LoaderFinished,
    MasterStarted,
    NewRound,
    NewWave,
    PathfindingToPlayerDone,
    PowerChanged,
    RotationsDone,
    ScreenFade,
    ScreenVisible,
    ZombiesKilledDuringWave
}
public interface IEventListener
{
    void EventDeclared(Event e);
}