using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinerSystem : MonoBehaviour, IEventListener
{
    public static MinerSystem Instance { get; private set; }
    private HashSet<Miner> miners = new();
    [SerializeField] private int baseAvailable = 100000000;
    [SerializeField] private int currentAvailableLeft;
    [SerializeField] private float waitPoints;
    [SerializeField, Range(0f, 10f)] private float speed = 10f;

    private void Awake()
    {
        Instance = this;
        EventSystem.AddEventListener(this, Event.RotationsDone);
        EventSystem.AddEventListener(this, Event.NewWave);
        EventSystem.AddEventListener(this, Event.NewRound);
        EventSystem.AddEventListener(this, Event.PowerChanged);
    }
    void Update()
    {
        if (currentAvailableLeft > 0 && speed > 0f)
        {
            waitPoints += Time.deltaTime;
            float waitPointsPerTick = 1f / speed;
            if (waitPoints > waitPointsPerTick)
            {
                int ticks = Mathf.FloorToInt(waitPoints / waitPointsPerTick);
                waitPoints -= waitPointsPerTick * ticks;
                Distribute(ticks);
            }
        }
        else
        {
            waitPoints = 0f;
        }
    }
    public void Track(Miner miner)
    {
        miners.Add(miner);
        Invoke(nameof(RecalculateAllEffratios), 1f);
    }
    public void Untrack(Miner miner)
    {
        miners.Remove(miner);
        Invoke(nameof(RecalculateAllEffratios), 1f);
    }
    private void Distribute(int amount)
    {
        amount = Mathf.Clamp(amount, 0, currentAvailableLeft);
        currentAvailableLeft -= amount;
        
        foreach (Miner miner in miners)
        {
            float expected = amount * miner.RealEffratio;
            int guaranteed = Mathf.FloorToInt(expected);
            float leftover = expected - guaranteed;  // From 0 to 1
            int finalAmount = guaranteed;
            if (leftover > Random.Range(0f, 1f)) finalAmount++;
            miner.Stored.Shitcoin += finalAmount;
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
    private float CalculateEffratioFor(Miner miner)
    {
        // Having miners in close proximity reduce the efficiency
        float divideBy = 1f;
        Vector2 pos = miner.transform.position;
        foreach (Miner m in miners)
        {
            if (m == miner) continue;
            if (!m.Powered) continue;
            Vector2 p = m.transform.position;  // cache position? modifiability--, speed++
            float distance = (p - pos).magnitude;  // sqrmagnitude for efficiency?
            if (distance > m.Range + miner.Range) continue;  // don't affect each other
            float occupiedHere = 1f-distance / (m.Range + miner.Range);
            divideBy += occupiedHere;
        }
        float ratio = 1f / divideBy;
        return ratio;
    }

    public void EventDeclared(Event e)
    {
        if (e == Event.RotationsDone)
        {
            RecalculateAllEffratios();
        }
        if (e == Event.NewRound)
        {
            Distribute(currentAvailableLeft);
        }
        if (e == Event.NewWave)
        {
            currentAvailableLeft = baseAvailable;
        }
        if (e == Event.PowerChanged)
        {
            Invoke(nameof(RecalculateAllEffratios), 1f);
        }
    }
}
