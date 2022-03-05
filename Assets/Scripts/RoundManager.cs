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
    [SerializeField] private int zombiesEachRound = 10;
    [SerializeField] private float minZombieDistance = 10f;
    [SerializeField] private float maxZombieDistance = 20f;

    private State state = State.warmup;
    [SerializeField] private float time;
    [SerializeField] private int round;

    // Start is called before the first frame update
    void Start()
    {
        
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
                StartCoroutine(SummonZombies());
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
                RoomManager.Instance.NextRound();
            }
        }

    }
    private IEnumerator SummonZombies()
    {
        for (int i = 0; i < zombiesEachRound; i++)
        {
            Zombie zombie = ZombiePool.Instance.Depool();
            float distance = Random.Range(minZombieDistance, maxZombieDistance);
            Vector2 direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            Vector2 offset = distance * direction;
            zombie.transform.position = Player.Instance.transform.position.Add(offset);
            yield return null;
        }
    }
}
