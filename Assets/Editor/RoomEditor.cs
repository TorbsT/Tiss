using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Room))]
public class RoomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Room room = (Room)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Start Rotation Animation"))
        {
            room.StartRotationAnimation();
        }
    }
}
