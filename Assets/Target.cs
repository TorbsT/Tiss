using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class Target : MonoBehaviour
    {
        public enum Discoverability
        {
            hidden,
            targetable,
            discoverable
        }
        private enum State
        {
            outOfBounds,
            unreadyRoom,
            readyRoom,
        }
        public float LocCheckDelay { get => locCheckDelay; set { locCheckDelay = value; } }
        public bool Targetable => discoverability == Discoverability.targetable || discoverability == Discoverability.discoverable;
        public bool Discoverable => discoverability == Discoverability.discoverable;
        public AllToOne AllToOne => allToOne;

        [Header("CONFIG")]
        [SerializeField] private float locCheckDelay;

        [Header("DEBUG")]
        [SerializeField] private bool targetable;
        [SerializeField] private float timeSinceLocCheck;
        [SerializeField] private bool outdated;
        [SerializeField] private Vector2Int loc;
        [SerializeField] private AllToOne allToOne;
        [SerializeField] private Discoverability discoverability;
        [SerializeField] private State state;
        private HashSet<ITargetSubscriber> subscribers = new();
        private RoomDweller dweller;
        private void Awake()
        {
            dweller = GetComponent<RoomDweller>();
        }
        private void OnEnable()
        {
            outdated = true;
            discoverability = Discoverability.hidden;
            RegisterTarget();
        }
        private void OnDisable()
        {
            PathfindingManager.Instance.RemoveTarget(this);
        }
        private void Update()
        {
            if (dweller == null || dweller.Room == null)
            {
                state = State.outOfBounds;
                allToOne = null;
            } else
            {
                AllToOne ato = PathfindingManager.Instance.GetAllToOne(dweller.Room);
                if (ato != allToOne)
                {
                    FireStateChanged();
                    allToOne = ato;
                    if (allToOne.CurrentState == AllToOne.State.empty)
                    {
                        allToOne.StartRecalculating();
                        state = State.unreadyRoom;
                    } else if (allToOne.CurrentState == AllToOne.State.running)
                    {
                        state = State.unreadyRoom;
                    } else if (allToOne.CurrentState == AllToOne.State.calculated)
                    {
                        state = State.readyRoom;
                    }
                }
            }
        }
        private void RegisterTarget()
        {
            if (PathfindingManager.Instance == null)
            {
                Invoke(nameof(RegisterTarget), 1f);
                return;
            }
            PathfindingManager.Instance.AddTarget(this);
        }
        private void AllChooseTarget()
        {
            PathfindingManager.Instance.AllChooseTarget();
        }
        public void SetDiscoverability(Discoverability newDiscoverability)
        {
            if (newDiscoverability == discoverability) return;
            if (newDiscoverability == Discoverability.hidden)
            {
                AllChooseTarget();
            } else if (newDiscoverability == Discoverability.targetable)
            {
                
            } else if (newDiscoverability == Discoverability.discoverable)
            {
                AllChooseTarget();
            }
            discoverability = newDiscoverability;
        }
        private void FireStateChanged()
        {
            foreach (ITargetSubscriber subscriber in subscribers)
            {
                subscriber.StateChanged();
            }
        }
        public void AddSubscriber(ITargetSubscriber subscriber)
        {
            subscribers.Add(subscriber);
        }
        public void RemoveSubscriber(ITargetSubscriber subscriber)
        {
            subscribers.Remove(subscriber);
        }
        public void MarkOutdated()
        {
            outdated = true;
            allToOne = null;
        }
    }
}

