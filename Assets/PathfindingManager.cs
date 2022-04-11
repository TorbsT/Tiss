using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class PathfindingManager : MonoBehaviour
    {
        public static PathfindingManager Instance { get; private set; }

        [SerializeField] private float allToOneFpsPriority;
        private Dictionary<Room, AllToOne> allToOnes = new();
        private HashSet<Target> targets = new();
        private HashSet<Zombie> zombies = new();

        private void Awake()
        {
            Instance = this;
        }
        public ICollection<Target> GetTargets()
        {
            HashSet<Target> result = new();
            foreach (Target t in targets)
            {
                result.Add(t);
            }
            return result;
        }
        public void AddTarget(Target target)
        {
            targets.Add(target);
        }
        public void RemoveTarget(Target target)
        {
            targets.Remove(target);
        }
        public void AddZombie(Zombie zombie)
        {
            zombies.Add(zombie);
        }
        public void RemoveZombie(Zombie zombie)
        {
            zombies.Remove(zombie);
        }
        public AllToOne GetAllToOne(Room room)
        {
            if (!allToOnes.ContainsKey(room))
            {
                AllToOne allToOne = room.gameObject.GetComponent<AllToOne>();
                allToOnes.Add(room, allToOne);
            }
            return allToOnes[room];
        }
        public void AllChooseTarget()
        {
            foreach (Zombie zombie in zombies)
            {
                zombie.Research();
            }
        }
        public void NewRound()
        {
            foreach (AllToOne ato in allToOnes.Values)
            {
                ato.RemoveData();
            }
            allToOnes = new();
            foreach (Target target in targets)
            {
                target.MarkOutdated();
            }
            AllChooseTarget();
        }
    }
}
