using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinerSystem : MonoBehaviour, IEventListener
{
    public static MinerSystem Instance { get; private set; }
    private HashSet<Miner> miners = new();

    private void Awake()
    {
        Instance = this;
        EventSystem.AddEventListener(this, Event.RotationsDone);
    }
    public void Track(Miner miner)
    {
        miners.Add(miner);
        RecalculateAllEffratios();
    }
    public void Untrack(Miner miner)
    {
        miners.Remove(miner);
        RecalculateAllEffratios();
    }
    public void FinishedRotating()
    {
        MakeMoreAvailable();
    }
    public void MakeMoreAvailable()
    {
        RecalculateAllEffratios();
        foreach (Miner miner in miners)
        {
            miner.AddToAvailable(Mathf.CeilToInt(miner.Effratio * miner.MaxProduction));
        }
        // TODO make coroutine
    }
    private void RecalculateAllEffratios()
    {
        foreach (Miner m in miners)
        {
            float effratio = CalculateEffratioFor(m);
            m.Effratio = effratio;
        }
    }
    public float CalculateEffratioFor(Miner miner)
    {
        // Having miners in close proximity reduce the efficiency
        float divideBy = 1f;
        Vector2 pos = miner.transform.position;
        foreach (Miner m in miners)
        {
            if (m == miner) continue;
            Vector2 p = m.transform.position;  // cache position? modifiability--, speed++
            float distance = (p - pos).magnitude;  // sqrmagnitude for efficiency?
            if (distance > m.Range + miner.Range) continue;  // don't affect each other
            float occupiedHere = 1f-distance / (m.Range + miner.Range);
            Debug.Log(occupiedHere);
            divideBy += occupiedHere;
        }
        float ratio = 1f / divideBy;
        Debug.Log(divideBy);
        return ratio;
    }

    public void EventDeclared(Event e)
    {
        if (e == Event.RotationsDone)
        {
            RecalculateAllEffratios();
        }
        if (e == Event.NewWave)
        {
            MakeMoreAvailable();
        }
    }
}
