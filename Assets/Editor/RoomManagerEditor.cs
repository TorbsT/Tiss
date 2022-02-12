using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomManager))]
public class RoomManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RoomManager manager = (RoomManager)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Next Round"))
        {
            manager.NextRound();
        }
    }
}
