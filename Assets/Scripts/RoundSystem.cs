using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pools;
using ExtensionMethods;

public class RoundSystem : MonoBehaviour, IEventListener
{
    private enum State
    {
        warmup,
        wave
    }
    public static RoundSystem Instance { get; private set; }
    public int Round => round;
    [SerializeField] private float warmupDuration = 10f;
    [SerializeField] private float waveDuration = 10f;

    private State state = State.warmup;
    [SerializeField] private float time;
    [SerializeField] private int round;

    private void Awake()
    {
        Instance = this;
        EventSystem.AddEventListener(this, Event.LoaderFinished);
    }
    // Update is called once per frame
    void Update()
    {
        RoundTimer timer = RoundTimer.Instance;

        time += Time.deltaTime;
        if (state == State.warmup)
        {
            timer.Time = Mathf.Ceil(warmupDuration - time);
            timer.LoadProgress = (warmupDuration - time) / warmupDuration;
            if (time >= warmupDuration)
            {
                time = 0f;
                state = State.wave;
                timer.ImageColor = Color.red;
                EventSystem.DeclareEvent(Event.NewWave);
            }
        } else if (state == State.wave)
        {
            timer.Time = Mathf.Ceil(waveDuration - time);
            timer.LoadProgress = time / waveDuration;
            if (time >= waveDuration)
            {
                time = 0f;
                state = State.warmup;
                timer.ImageColor = Color.green;
                round++;
                RoundChanged();
            }
        }
    }
    private void RoundChanged()
    {
        EventSystem.DeclareEvent(Event.NewRound);
    }
    public void EventDeclared(Event e)
    {
        if (e == Event.LoaderFinished)
        {
            EventSystem.RemoveListener(this);
            RoundChanged();
        }
    }
}
