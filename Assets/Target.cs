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
            PathfindingSystem.Instance.RemoveTarget(this);
        }
        private void Update()
        {
            if (dweller == null || dweller.Room == null)
            {
                state = State.outOfBounds;
                allToOne = null;
            } else
            {
                allToOne = dweller.Room.GetComponent<AllToOne>();
                if (allToOne == null)
                {
                    state = State.unreadyRoom;
                    PathfindingSystem.Instance.RequestAllToOne(this);
                } else
                {
                    state = State.readyRoom;
                }
            }
        }
        private void RegisterTarget()
        {
            if (PathfindingSystem.Instance == null)
            {
                Invoke(nameof(RegisterTarget), 1f);
                return;
            }
            PathfindingSystem.Instance.AddTarget(this);
        }
        public void SetDiscoverability(Discoverability newDiscoverability)
        {
            if (newDiscoverability == discoverability) return;
            if (newDiscoverability == Discoverability.hidden)
            {
                PathfindingSystem.Instance.RemoveTarget(this);
            } else if (newDiscoverability == Discoverability.targetable)
            {
                PathfindingSystem.Instance.AddTarget(this);
            } else if (newDiscoverability == Discoverability.discoverable)
            {
                PathfindingSystem.Instance.AddTarget(this);
            }
            discoverability = newDiscoverability;
        }
        public void MarkOutdated()
        {
            outdated = true;
            allToOne = null;
        }
    }
}

