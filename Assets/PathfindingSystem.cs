using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class PathfindingSystem : MonoBehaviour, IEventListener
    {
        public static PathfindingSystem Instance { get; private set; }

        [SerializeField] private float allToOneFpsPriority;
        private Dictionary<Room, AllToOne> allToOnes = new();
        private HashSet<Target> targets = new();
        private HashSet<Zombie> zombies = new();
        private HashSet<Target> waitingTargets = new();
        private Dictionary<Target, AllToOne> latestTargetToATOs = new();

        private void Awake()
        {
            Instance = this;
            EventSystem.AddEventListener(this, Event.RotationsDone);
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
        public void AllToOneDone(AllToOne ato)
        {
            if (Player.Instance.GetComponent<RoomDweller>().Room == ato.GetComponent<Room>())
            {
                EventSystem.DeclareEvent(Event.PathfindingToPlayerDone);
            }
            foreach (Target t in waitingTargets)
            {
                if (t.GetComponent<RoomDweller>().Room == ato.GetComponent<Room>())
                {
                    waitingTargets.Remove(t);
                    latestTargetToATOs[t] = ato;
                    AllChooseTarget();
                    break;
                }
            }
            
        }
        public void RequestAllToOne(Target target)
        {
            waitingTargets.Add(target);
            CreateATO(target);
        }
        public AllToOne LatestAllToOne(Target target)
        {
            // unsafe
            if (!latestTargetToATOs.ContainsKey(target)) return null;
            return latestTargetToATOs[target];
        }
        public void AllChooseTarget()
        {
            foreach (Zombie zombie in zombies)
            {
                zombie.Research();
            }
        }

        public void Doshitagain()
        {
            foreach (AllToOne ato in allToOnes.Values)
            {
                Destroy(ato);
            }
            allToOnes = new();

            foreach (Target target in targets)
            {
                CreateATO(target);
            }
        }

        public void EventDeclared(Event e)
        {
            if (e == Event.RotationsDone)
            {
                Doshitagain();
            }
        }

        private void CreateATO(Target target)
        {
            if (target.GetComponent<RoomDweller>().Room.GetComponent<AllToOne>() != null) return;
            AllToOne ato = target.GetComponent<RoomDweller>().Room.gameObject.AddComponent<AllToOne>();
            allToOnes.Add(ato.GetComponent<Room>(), ato);
        }
    }
}
