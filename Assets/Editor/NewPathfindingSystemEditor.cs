using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Pathfinding
{
    [CustomEditor(typeof(WorldGraphSystem))]
    public class NewPathfindingSystemEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            WorldGraphSystem p = (WorldGraphSystem)target;

            DrawDefaultInspector();

            if (GUILayout.Button("Recalculate"))
            {
                p.Recalculate();
            }
        }
    }

}