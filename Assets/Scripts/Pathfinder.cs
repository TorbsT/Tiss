using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;

namespace Pathfinding
{
    public class Pathfinder : MonoBehaviour
    {
        private enum State
        {
            noTarget,
            targetHasNoAllToOne,
            outOfBounds,
            noPathAvailable,
            inSameRoom,
            navigatingToNextRoom,
        }
        public bool StayStill => subgoal == transform;

        [Header("CONFIG")]
        [SerializeField] private float roomReachedDistance = 1f;
        [SerializeField] private float reachedCheckDelay = 1f;

        [Header("DEBUG")]
        private Target target;
        [SerializeField] private Transform subgoal;
        [SerializeField] private Room currentRoom;
        [SerializeField] private Vector2Int currentLoc;
        [SerializeField] private bool outdated;
        [SerializeField] private State state = State.noTarget;
        [SerializeField] private float subGoalDistanceRequirement;
        [SerializeField] private float timeSinceReachedCheck;
        private HashSet<IPathfinderListener> listeners = new();

        private bool SubGoalReached => transform.position.Subtract(subgoal.position).magnitude < subGoalDistanceRequirement;

        private void OnEnable()
        {
            outdated = true;
            listeners = new();
            foreach (IPathfinderListener comp in GetComponents<IPathfinderListener>())
            {
                listeners.Add(comp);
            }
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (target == null)
            {
                state = State.noTarget;
                return;
            }

            timeSinceReachedCheck += Time.deltaTime;
            if (timeSinceReachedCheck > reachedCheckDelay)
            {
                if (SubGoalReached) outdated = true;
                timeSinceReachedCheck = 0f;
            }

            if (outdated || currentRoom == null)
            {  // Target moved, a rotation took place, or subgoal was reached
                
                currentLoc = RoomManager.Instance.PosToLoc(transform.position);
                currentRoom = RoomManager.Instance.PosToRoom(transform.position);
                Transform newSubgoal = null;
                if (currentRoom == null)
                {
                    state = State.outOfBounds;
                    newSubgoal = target.transform;
                    subGoalDistanceRequirement = 0f;
                } else
                {
                    Node node = target.AllToOne.GetNode(currentRoom);
                    if (node == null)
                    {  // no path available
                        state = State.noPathAvailable;
                        newSubgoal = target.transform;
                        subGoalDistanceRequirement = 0f;
                    } else
                    {
                        ICollection<Room> closerNeighbours = node.CloserNeighbours;
                        if (closerNeighbours.Count == 0)
                        {  // In same room
                            state = State.inSameRoom;
                            newSubgoal = target.transform;
                            subGoalDistanceRequirement = 0f;
                        }
                        else
                        {  // Has some closer rooms
                            state = State.navigatingToNextRoom;
                            subGoalDistanceRequirement = roomReachedDistance;
                            int chosenIndex = Random.Range(0, closerNeighbours.Count);
                            int i = 0;
                            foreach (Room r in closerNeighbours)
                            {  // Could be optimized with lists, but would have to change some shit
                                if (i == chosenIndex)
                                {
                                    newSubgoal = r.transform;
                                    break;
                                }
                                i++;
                            }
                        }
                    }
                    
                }
                if (newSubgoal != subgoal)
                {
                    foreach (IPathfinderListener listener in listeners)
                    {
                        listener.NewSubgoal(subgoal, newSubgoal);
                    }
                    subgoal = newSubgoal;
                }
                outdated = false;
            }
        }
        public void SetTarget(Target target)
        {
            this.target = target;
        }
    }
}

