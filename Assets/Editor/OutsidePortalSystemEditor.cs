using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OutsidePortalSystem))]
public class OutsidePortalSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        OutsidePortalSystem p = (OutsidePortalSystem)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Spawn"))
        {
            p.Spawn();
        }
    }
}