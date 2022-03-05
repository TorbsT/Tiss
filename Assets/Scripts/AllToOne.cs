using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class AllToOne : MonoBehaviour
    {  // Attached to a room. Must be MB because of coroutines
        public enum State
        {
            empty,
            running,
            calculated
        }
        public float FpsPriority { get => fpsPriority; set { fpsPriority = value; } }
        public State CurrentState => state;
        [SerializeField] private float fpsPriority;
        [SerializeField] private float calculationTime;
        private Dictionary<Room, Node> nodes;
        private Room targetRoom;
        private Coroutine coroutine;
        [SerializeField] private State state;
        [SerializeField, Range(1f, 10f)] private float gizmoBoxSize;
        private float calculationStartTime;

        private void Awake()
        {
            targetRoom = GetComponent<Room>();
        }
        private void OnDrawGizmosSelected()
        {
            if (state != State.calculated) return;
            foreach (Room room in nodes.Keys)
            {
                Node node = nodes[room];
                foreach (Room r in node.CloserNeighbours)
                {
                    float steps = node.StepsFromTarget;
                    Gizmos.color = new Color(1f-steps/10f, 1f-steps/10f, 0f);
                    Gizmos.DrawLine(room.transform.position, r.transform.position);
                    Gizmos.DrawCube(room.transform.position, Vector3.one*gizmoBoxSize);
                }
            }
        }
        public Node GetNode(Room room)
        {
            if (!nodes.ContainsKey(room)) return null;
            return nodes[room];
        }
        public void RemoveData()
        {
            if (state == State.running) StopCoroutine(coroutine);
            nodes = new();
            state = State.empty;
        }
        public void StartRecalculating()
        {
            if (state != State.empty) RemoveData();
            coroutine = StartCoroutine(DijkstraRoutine());
        }
        private IEnumerator DijkstraRoutine()
        {
            state = State.running;
            calculationStartTime = Time.time;
            Waiter waiter = new(1f / fpsPriority);
            Dictionary<Room, Node> newNodes = new();

            Queue<Node> nodesToExplore = new();
            Node firstNode = new(targetRoom);
            firstNode.StepsFromTarget = 0;
            nodesToExplore.Enqueue(firstNode);
            newNodes.Add(targetRoom, firstNode);
            while (nodesToExplore.Count > 0)
            {
                Node node = nodesToExplore.Dequeue();
                Room room = node.Room;
                
                foreach (Room r in node.Room.GetAccessibleRooms())
                {
                    if (!newNodes.ContainsKey(r))
                    {
                        Node n = new(r);
                        n.AddCloserNeighbour(room);
                        n.StepsFromTarget = node.StepsFromTarget + 1;
                        newNodes.Add(r, n);
                        nodesToExplore.Enqueue(n);
                    } else
                    {
                        Node n = newNodes[r];
                        if (n.StepsFromTarget == node.StepsFromTarget + 1)
                        {
                            n.AddCloserNeighbour(room);
                        }
                    }

                    if (waiter.CheckTime()) yield return null;
                }
            }

            calculationTime = Time.time - calculationStartTime;
            nodes = newNodes;
            state = State.calculated;
        }
    }
    public class Node
    {
        public int StepsFromTarget { get => stepsFromTarget; set { stepsFromTarget = value; } }
        public Room Room => room;
        public ICollection<Room> CloserNeighbours => closerNeighbours;

        private Room room;
        private HashSet<Room> closerNeighbours = new();
        private int stepsFromTarget;

        public Node(Room room)
        {
            this.room = room;
        }

        public void AddCloserNeighbour(Room room)
        {
            closerNeighbours.Add(room);
        }
        public void FlushCloserNeighbours()
        {
            closerNeighbours = new();
        }
    }
}

