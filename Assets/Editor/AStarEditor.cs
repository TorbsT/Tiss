using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Pathfinding
{
    [CustomEditor(typeof(AStar))]
    public class AStarEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            AStar astar = (AStar)target;

            DrawDefaultInspector();

            if (GUILayout.Button("Compute"))
            {
                astar.Compute();
            }
        }
    }

}