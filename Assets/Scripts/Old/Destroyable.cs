using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour
{
    private Queue<string> stackTraces = new();

    [SerializeField] private bool debug;
    [SerializeField] private EzPools.Pool connected;
    private bool destroyed;
    private bool quitting;

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
        if (QuittingSystem.Quitting) return;
        if (connected == null)
        {
            if (Application.isEditor) DestroyImmediate(gameObject);
            else GameObject.Destroy(gameObject);
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
