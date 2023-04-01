using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindSystem : MonoBehaviour
{
    private struct Request
    {
        public Vector2 pos;
        public float range;
        public float speed;
        public float rewindDuration;
    }
    public static RewindSystem Instance { get; private set; }

    public bool RewindHP => rewindHP;

    [SerializeField] private bool rewindHP;
    [SerializeField, Range(1, 100)] private int waitFrames;
    private HashSet<Rewinder> rewinders = new();
    private Stack<Request> requests = new();

    private int framesWaited;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        // Check requests and start replays
        foreach (Request request in requests)
        {
            Vector2 pos = request.pos;
            float range = request.range;
            Debug.Log("Process request");
            foreach (Collider2D collider in Physics2D.OverlapCircleAll(pos, range))
            {
                Debug.Log("Found collider");
                Rewinder rewinder = collider.GetComponent<Rewinder>();
                if (rewinder != null)
                {
                    Debug.Log("Found rewinder");
                    rewinder.Record(rewinder.LocalTime);
                    rewinder.Speed = request.speed;
                    rewinder.Rewinding = true;
                    rewinder.RewindFinishTime = Mathf.Max(rewinder.RewindFinishTime, Time.fixedTime + request.rewindDuration);
                }
            }
        }
        if (requests.Count > 0) requests = new();

        // Stop rewind for those that are finished
        foreach (Rewinder r in rewinders)
        {
            if (r.Rewinding && Time.fixedTime >= r.RewindFinishTime)
            {
                r.Rewinding = false;
                r.RemoveFutureFrames(r.LocalTime);
            }
        }

        // Rewind
        float timeSinceLastFrame = Time.fixedDeltaTime;
        foreach (Rewinder rewinder in rewinders)
        {
            if (rewinder.Rewinding)
            {
                rewinder.Replay(rewinder.LocalTime);
                rewinder.LocalTime = Mathf.Max(0f, rewinder.LocalTime - timeSinceLastFrame*rewinder.Speed);
            }
        }

        framesWaited++;
        bool recordThisFrame = framesWaited >= waitFrames;
        if (recordThisFrame) framesWaited = 0;
            
        // Update local time (and record frame sometimes)
        foreach (Rewinder rewinder in rewinders)
        {
            if (!rewinder.Rewinding)
            {
                if (recordThisFrame) rewinder.Record(rewinder.LocalTime);
                rewinder.LocalTime += timeSinceLastFrame;
            }
        }
    }

    public void StartRewind(Vector2 pos, float range, float speed, float duration)
    {
        Request r = new();
        r.pos = pos;
        r.range = range;
        r.speed = speed;
        r.rewindDuration = duration;
        requests.Push(r);
    }
    public void Track(Rewinder r)
    {
        rewinders.Add(r);
    }
    public void Untrack(Rewinder r)
    {
        rewinders.Remove(r);
    }
}
