using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseSystem : MonoBehaviour
{
    public static PauseSystem Instance { get; private set; }

    [SerializeField, Range(0f, 1f)] private float pauseSpeed = 1f;
    [SerializeField, Range(0f, 1f)] private float resumeSpeed = 1f;
    [SerializeField] private bool paused;
    [SerializeField] private float timeScale;

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        timeScale = Time.timeScale;
        if (paused) timeScale -= pauseSpeed;
        else timeScale += resumeSpeed;
        timeScale = Mathf.Clamp(timeScale, 0f, 1f);
        Time.timeScale = timeScale;
    }
    public void Pause()
    {
        paused = true;
    }
    public void Resume()
    {
        paused = false;
    }
}
