using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class PathfindingSystem : MonoBehaviour, IEventListener
    {
        public static PathfindingSystem Instance { get; private set; }

        [SerializeField] private float allToOneFpsPriority;
        [SerializeField] private int targetCount;
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
            Debug.Log("Added " + target.gameObject);
            targetCount = targets.Count;
        }
        public void RemoveTarget(Target target)
        {
            targets.Remove(target);
            Debug.Log("Removed " + target.gameObject);
            targetCount = targets.Count;
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
            /*
            HashSet<Target> tgs = new();
            foreach (Target t in waitingTargets)
            {
                Debug.Log(t + " is waiting");
                tgs.Add(t);
            }
            /*
            foreach (Target t in tgs)
            {
                if (t.GetComponent<RoomDweller>().Room == ato.GetComponent<Room>())
                {
                    Debug.Log("Match " + t);
                    waitingTargets.Remove(t);
                    latestTargetToATOs[t] = ato;
                }
            }*/
            foreach (Target t in ato.transform.GetComponentsInChildren<Target>())
            {
                latestTargetToATOs[t] = ato;
            }
            AllChooseTarget();

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
            AllToOne ato = target.GetComponent<RoomDweller>().Room.GetComponent<AllToOne>();
            if (ato != null)
            {
                AllToOneDone(ato);  // bad probably
                return;
            }
            ato = target.GetComponent<RoomDweller>().Room.gameObject.AddComponent<AllToOne>();
            allToOnes.Add(ato.GetComponent<Room>(), ato);
        }
    }
}
