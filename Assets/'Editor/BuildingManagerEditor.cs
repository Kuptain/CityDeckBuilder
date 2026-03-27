using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;


[CustomEditor(typeof(BuildingManager))]
public class BuildingManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if(GUILayout.Button("Open Editor")) 
        {
            BuildingEditorWindow.Open(FindAnyObjectByType<BuildingManager>().container);
        }
        base.OnInspectorGUI();
    }
}
