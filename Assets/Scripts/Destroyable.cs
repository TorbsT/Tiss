using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour
{
    private Queue<string> stackTraces = new();
    private EzPools.Pool connected;

    [SerializeField] private bool debug;
    private bool destroyed;

    public void JustInstantiated(EzPools.Pool pool)
    {
        connected = pool;
        Log("INSTANTIATED");
    }
    public void JustDepooled()
    {
        Log("DEPOOLED");

        if (!destroyed)
        {
            Debug.LogWarning(gameObject+" Invalid state on JustDepooled()");
            foreach (string trace in stackTraces)
            {
                Debug.LogWarning(trace);
            }
        }
        destroyed = false;
    }
    public void Destroy()
    {
        if (connected == null)
        {
            GameObject.Destroy(gameObject);
            Log("DESTROYED");
        } else
        {
            connected.Enpool(gameObject);
            Log("ENPOOLED");
        }

        if (destroyed)
        {
            Debug.LogWarning(gameObject+" Invalid state on Destroy()");
            foreach (string trace in stackTraces)
            {
                Debug.LogWarning(trace);
            }
        }
        destroyed = true;
    }
    private void Log(string mode)
    {
        if (debug)
            stackTraces.Enqueue(mode+ " ("+Time.frameCount+"): " + Environment.StackTrace);
    }
}
