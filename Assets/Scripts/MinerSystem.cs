using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinerSystem : MonoBehaviour
{
    public static MinerSystem Instance { get; private set; }
    private HashSet<Miner> miners = new();
    private static HashSet<Miner> preMiners = new();

    private void Awake()
    {
        Instance = this;
        foreach (Miner m in preMiners)
        {
            miners.Add(m);
        }
    }
    public static void Track(Miner miner)
    {
        if (Instance != null)
        {
            Instance.miners.Add(miner);
        } else
        {
            preMiners.Add(miner);
        }
    }
    public void Untrack(Miner miner)
    {
        miners.Remove(miner);
    }
    public void MakeMoreAvailableDelayed()
    {
        Invoke(nameof(MakeMoreAvailable), 3f);  //hohohoho this is so bad
    }
    private void MakeMoreAvailable()
    {
        foreach (Miner miner in miners)
        {
            float effratio = CalculateEffratioFor(miner);
            miner.Effratio = effratio;
            miner.AddToAvailable(Mathf.CeilToInt(effratio * miner.MaxProduction));
        }
        // TODO make coroutine
    }
    public float CalculateEffratioFor(Miner miner)
    {
        // Having miners in close proximity reduce the efficiency
        Debug.Log("Æ");
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
}
