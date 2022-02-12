using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Routine : MonoBehaviour
{
    public bool Running { get => running; }

    [SerializeField] private bool running;
    
    private Coroutine coroutine;
    
    protected void TryRun()
    {
        if (running)
        {
            Debug.LogWarning(this + " is already running");
        }
        else
        {
            coroutine = StartCoroutine(Run());
        }
    }
    protected void TryStop()
    {
        if (!Running)
        {
            Debug.LogWarning(this + " is not running");
        }
        else
        {
            StopCoroutine(coroutine);
        }
    }
    protected abstract IEnumerator Run();
}
