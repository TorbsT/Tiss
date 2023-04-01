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
    [SerializeField] private Target target;
    [SerializeField] private bool running;
    private float rdm;
    private ICollection<ITargetChooserListener> listeners = new HashSet<ITargetChooserListener>();

    private void Awake()
    {
        foreach (ITargetChooserListener comp in GetComponents<ITargetChooserListener>())
        {
            listeners.Add(comp);
        }
        rdm = Random.Range(0f, 1f);
    }
    private void OnEnable()
    {

    }
    private void OnDisable()
    {
        
    }
    private void Update()
    {
        if (!running)
        {
            if (target == null || !target.isActiveAndEnabled || !target.Targetable) Research();
        }
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
        currentRoom = SquareRoomSystem.Instance.PosToRoom(transform.position);

        List<Target> closestTargets = new();
        if (currentRoom != null)
        {
            int closestStepsFromTarget = int.MaxValue;
            ICollection<Target> tgts = PathfindingSystem.Instance.GetTargets();
            //Debug.Log(gameObject + " Æ" + tgts.Count);
            foreach (Target t in tgts)
            {
                //Debug.Log(gameObject + " 2 ");
                yield return null;
                //Debug.Log(gameObject + " 3 ");
                if (!t.Discoverable) continue;
                //Debug.Log(gameObject + " 4 ");
                AllToOne ato = PathfindingSystem.Instance.LatestAllToOne(t);
                if (ato == null) Debug.Log(t + " does not have an ATO ");
                if (ato == null) continue;
                //Debug.Log(gameObject + " 5 ");
                Node node = ato.GetNode(currentRoom);
                if (node == null) continue;
                //Debug.Log(gameObject + " 6 ");
                int stepsFromTarget = node.StepsFromTarget;
                if (stepsFromTarget <= closestStepsFromTarget)
                {
                    if (stepsFromTarget < closestStepsFromTarget) closestTargets = new();
                    closestTargets.Add(t);
                    closestStepsFromTarget = stepsFromTarget;
                    //Debug.Log(closestTargets.Count + " Æ" + stepsFromTarget + " " + gameObject + " " + t.gameObject);
                }
            }
        }


        Target closestTarget = null;
        Debug.Log(closestTargets.Count);
        int chosenIndex = Mathf.FloorToInt(rdm * closestTargets.Count);
        if (chosenIndex < 0 || chosenIndex >= closestTargets.Count) Debug.Log("rdm " + rdm + ", " + chosenIndex + " " + closestTargets.Count);
        closestTarget = closestTargets[chosenIndex];
        // If the zombie is out of bounds,
        // or if no rooms can be accessed from its current room:
        // choose the target with least air distance
        if (closestTarget == null)
        {
            float shortestDistance = float.MaxValue;
            foreach (Target t in PathfindingSystem.Instance.GetTargets())
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
