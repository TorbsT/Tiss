using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public interface ITargetChooserListener
    {
        void ChoseNewTarget(Target current, Target newTarget);
    }

}
