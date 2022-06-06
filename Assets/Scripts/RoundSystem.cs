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
    public float CurrentWaveDuration => waveDuration;
    private RoundTimer timer => RoundTimer.Instance;
    private RoundInfoUI info => RoundInfoUI.Instance;
    private RoundDescUI desc => RoundDescUI.Instance;
    [SerializeField] private float warmupDuration = 10f;
    [SerializeField] private float waveDuration = 10f;

    private State state = State.warmup;
    [SerializeField] private float time;
    [SerializeField] private float timerScalar = 1f;
    [SerializeField] private int round;
    private bool started;

    private void Awake()
    {
        Instance = this;
        EventSystem.AddEventListener(this, Event.LoaderFinished);
        EventSystem.AddEventListener(this, Event.ZombiesKilledDuringWave);
    }
    void Start()
    {
        SetColor(Color.green);
        info.Text = "Round "+round;
        desc.Text = "Prepare your defences";
    }
    // Update is called once per frame
    void Update()
    {
        if (!started) return;
        time += Time.deltaTime*timerScalar;
        if (state == State.warmup)
        {
            timer.Time = Mathf.Ceil(warmupDuration - time);
            timer.LoadProgress = (warmupDuration - time) / warmupDuration;
            if (time >= warmupDuration)
            {
                time = 0f;
                state = State.wave;
                SetColor(Color.red);
                EventSystem.DeclareEvent(Event.NewWave);
                info.Text = "Wave " + round;
                desc.Text = "Survive";
            }
        } else if (state == State.wave)
        {
            timer.Time = Mathf.Ceil(waveDuration - time);
            timer.LoadProgress = time / waveDuration;
            if (time >= waveDuration)
            {
                round++;
                time = 0f;
                state = State.warmup;
                info.Text = "Round " + round;
                desc.Text = "Prepare your defences";
                SetColor(Color.green);
                RoundChanged();
            }
        }
    }
    private void SetColor(Color c)
    {
        timer.ImageColor = c;
        timer.TextColor = c;
        info.Color = c;
        desc.Color = c;
    }
    private void RoundChanged()
    {
        timerScalar = 1f;
        EventSystem.DeclareEvent(Event.NewRound);
    }
    public void EventDeclared(Event e)
    {
        if (e == Event.LoaderFinished)
        {
            EventSystem.RemoveEventListener(this, Event.LoaderFinished);
            started = true;
            RoundChanged();
        } else if (e == Event.ZombiesKilledDuringWave)
        {
            timerScalar = 10f;
            desc.Text = "All zombies eliminated";
            desc.Color = Color.green;
        }
    }
}
