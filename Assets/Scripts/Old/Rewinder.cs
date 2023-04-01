using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rewinder : MonoBehaviour
{
    [System.Serializable]
    public struct Frame
    {
        public float localTime;
        public Vector2 pos;
        public Quaternion rot;
        public float health;
    }

    private bool RewindHP => RewindSystem.Instance.RewindHP;
    public float LocalTime { get => localTime; set { localTime = value; } }

    public float Speed { get => speed; set { speed = value; } }
    public bool Rewinding { get => rewinding; set { rewinding = value; } }
    public float RewindFinishTime { get => rewindFinishTime; set { rewindFinishTime = value; } }

    //private Frame[] frames;
    [SerializeField] private List<Frame> frames = new();
    [SerializeField] private bool rewinding;
    private float speed;
    [SerializeField] private float localTime;
    private float rewindFinishTime;


    private int latestIndex;
    // cached
    private HP hp;

    private void Awake()
    {
        hp = GetComponent<HP>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnEnable()
    {
        frames = new();
        RewindSystem.Instance.Track(this);
    }
    private void OnDisable()
    {
        RewindSystem.Instance.Untrack(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Record(float localTime)
    {
        Frame frame = new();
        frame.localTime = localTime;
        frame.pos = transform.position;
        frame.rot = transform.rotation;
        if (hp != null) frame.health = hp.Health;
        frames.Add(frame);
        latestIndex = frames.Count-1;
    }
    public void Replay(float localTime)
    {
        latestIndex = GetCorrectIndex(localTime);
        if (latestIndex < 0)
        {
            Debug.LogError("What");
        } else if (latestIndex == 0)
        {
            ReplayIndex(latestIndex);
        } else
        {
            float lerp = Mathf.InverseLerp(frames[latestIndex - 1].localTime, frames[latestIndex].localTime, localTime);
            ReplayIndexes(latestIndex, lerp);
        }
    }

    public void RemoveFutureFrames(float fromLocalTime)
    {
        // OPT use binary search
        int i;
        int entriesToRemove = 0;
        for (i = frames.Count-1; i >= 0; i--)
        {
            if (frames[i].localTime < fromLocalTime) break;
            entriesToRemove++;
        }
        int startIndex = frames.Count - entriesToRemove;
        if (startIndex >= frames.Count) return;
        frames.RemoveRange(startIndex, entriesToRemove);
    }


    private void ReplayIndex(int index)
    {
        Frame frame = frames[index];
        transform.SetPositionAndRotation(frame.pos, frame.rot);
        if (hp != null && RewindHP)
        {
            hp.Set(frame.health);
        }
    }
    private void ReplayIndexes(int latestIndex, float lerp)
    {
        Frame a = frames[latestIndex-1];
        Frame b = frames[latestIndex];

        Vector2 lerpedPos = Vector2.Lerp(a.pos, b.pos, lerp);
        Quaternion lerpedRot = Quaternion.Lerp(a.rot, b.rot, lerp);
        transform.SetPositionAndRotation(lerpedPos, lerpedRot);
        if (hp != null && RewindHP)
        {
            float lerpedHP = a.health + (b.health - a.health) * lerp;
            hp.Set(lerpedHP);
        }
    }
    private int GetCorrectIndex(float localTime)
    {
        if (localTime <= frames[0].localTime) return 0;
        if (localTime >= frames[^1].localTime) return frames.Count - 1;

        // l will now always be r - 1. Ignore it now

        int r = latestIndex;

        while (localTime <= frames[r].localTime)
        {
            r--;
        }
        while (localTime >= frames[r].localTime)
        {
            r++;
        }
        /*
        if (latestIndex > 0)
            // Adjust backwards
            for (int i = latestIndex-1; i >= 0; i--)
            {
                if (IsBestIndex(localTime, i))
                {
                    latestIndex = i;
                    break;
                }
            }
        if (latestIndex < frames.Count)

            // Adjust forwards
            for (int i = latestIndex; i < frames.Count; i++)
            {
                if (IsBestIndex(localTime, i))
                {
                    latestIndex = i;
                    break;
                }
            }
        */
        
        Debug.Log("Best index for " + localTime + " is " + r);
        return r;
    }
    private bool IsBestIndex(float localTime, int tryIndex)
    {
        if (tryIndex == 0)
        {
            return frames[tryIndex].localTime >= localTime;
        } else if (tryIndex == frames.Count-1)
        {
            return frames[tryIndex].localTime <= localTime;
        } else
        {
            return frames[tryIndex].localTime <= localTime && frames[tryIndex - 1].localTime >= localTime;
        }

    }
}
