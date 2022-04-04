using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="SO/ShopPool")]
public class ShopPoolObject : ScriptableObject
{
    [System.Serializable]
    public class Spawn
    {
        public string Group => group;
        public Item Item => item;
        public int Quantity => quantity;
        public int Cost => cost;
        public string Title => title;
        public string Desc => desc;

        [SerializeField] private string title;
        [SerializeField] private string group;
        [SerializeField] private string desc;
        [SerializeField] private Item item;
        [SerializeField, Range(1, 9999)] private int quantity;
        [SerializeField, Range(1, 999)] private int cost;
    }
    public int Seed { get => seed; set { seed = value; } }

    [SerializeField] private List<Spawn> spawns;
    [SerializeField] private int seed = -1;

    private System.Random random;

    public ICollection<Spawn> Generate(int count)
    {
        random = new(seed);
        HashSet<Spawn> result = new();
        int available = spawns.Count;
        for (int i = 0; i < spawns.Count; i++)
        {
            if (available <= 0) break;
            if (random.NextDouble() <= (float)count / available)
            {
                count--;
                result.Add(spawns[i]);
            }
            available--;
        }
        Debug.Log(result.Count + " " + available);
        return result;
    }
}
