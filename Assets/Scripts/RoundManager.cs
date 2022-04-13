using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pools;
using ExtensionMethods;

public class RoundManager : MonoBehaviour
{
    private enum State
    {
        warmup,
        wave
    }

    [SerializeField] private float warmupDuration = 10f;
    [SerializeField] private float waveDuration = 10f;
    [SerializeField] private AnimationCurve zombiesPerRound;

    private State state = State.warmup;
    [SerializeField] private float time;
    [SerializeField] private int round;

    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(PortalSpawning), 1f);  // BAD
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
                PortalSystem.Instance.StartSpawningZombies();
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

                foreach (Zombie zombie in FindObjectsOfType<Zombie>())
                {
                    zombie.GetComponent<HP>().Set(0f);
                }

                RoomManager.Instance.NextRound();
                Invoke(nameof(PortalSpawning), 1f);  // BAD
            }
        }
    }
    private void PortalSpawning()
    {
        int zombiesToSpawn = Mathf.FloorToInt(zombiesPerRound.Evaluate(round));
        PortalSystem.Instance.ZombiesToSpawn = zombiesToSpawn;
        PortalSystem.Instance.StartSpawningPortals();
    }
}
