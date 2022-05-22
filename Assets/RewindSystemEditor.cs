using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RewindSystem))]
public class RewindSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RewindSystem sys = (RewindSystem)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Rewind all 10 seconds"))
        {
            sys.StartRewind(Vector2.zero, 200f, 10f, 10f);
        }
    }
}
