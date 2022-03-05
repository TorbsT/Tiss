using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class TargetChooser : MonoBehaviour
{
    private enum State
    {
        inactive,

    }

    public bool Running => running;
    public Target Target => target;

    private Room currentRoom;
    private Target target;
    [SerializeField] private bool running;
    private ICollection<ITargetChooserListener> listeners = new HashSet<ITargetChooserListener>();

    private void Awake()
    {
        
    }
    private void OnEnable()
    {
        listeners = new HashSet<ITargetChooserListener>();
        foreach (ITargetChooserListener comp in GetComponents<ITargetChooserListener>())
        {
            listeners.Add(comp);
        }
    }
    private void OnDisable()
    {
        
    }
    public void Research()
    {
        if (running) StopAllCoroutines();
        StartCoroutine(ChooseTargetRoutine());
    }
    private IEnumerator ChooseTargetRoutine()
    {
        target = null;
        running = true;
        currentRoom = RoomManager.Instance.PosToRoom(transform.position);
        Target closestTarget = null;
        if (currentRoom != null)
        {
            int closestStepsFromTarget = int.MaxValue;
            foreach (Target t in PathfindingManager.Instance.GetTargets())
            {
                yield return null;
                if (!t.Discoverable) continue;
                AllToOne ato = t.AllToOne;
                if (ato == null) continue;
                Node node = ato.GetNode(currentRoom);
                if (node == null) continue;
                int stepsFromTarget = node.StepsFromTarget;
                if (stepsFromTarget < closestStepsFromTarget)
                {
                    closestStepsFromTarget = stepsFromTarget;
                    closestTarget = t;
                }
            }
        }

        // If the zombie is out of bounds,
        // or if no rooms can be accessed from its current room:
        // choose the target with least air distance
        if (closestTarget == null)
        {
            float shortestDistance = float.MaxValue;
            foreach (Target t in PathfindingManager.Instance.GetTargets())
            {
                yield return null;
                float distance = (t.transform.position - transform.position).sqrMagnitude;
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestTarget = t;
                }
            }
        }

        if (closestTarget == null)
        {
            // Try again later
            Invoke(nameof(Research), 1f);
        } else
        {
            running = false;
            foreach (ITargetChooserListener listener in listeners)
            {
                listener.ChoseNewTarget(target, closestTarget);
            }
            target = closestTarget;
        }
    }
}
