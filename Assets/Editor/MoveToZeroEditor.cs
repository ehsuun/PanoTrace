using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MovetoZero))]
public class MoveToZeroEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MovetoZero myScript = (MovetoZero)target;
        if (GUILayout.Button("Move praent to make this zero"))
        {
            myScript.MoveThistoZero();
        }
    }
}