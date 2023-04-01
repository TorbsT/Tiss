using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPathfinderListener
{
    void NewSubgoal(Transform old, Transform newGoal);
}
