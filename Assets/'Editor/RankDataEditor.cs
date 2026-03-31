using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;


[CustomEditor(typeof(BuildingData))]
public class RankDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SerializedProperty prop = serializedObject.FindProperty("buildingName");
        EditorGUILayout.PropertyField(prop, false);
        while (prop.NextVisible(false))
        {
            if(prop.name == "craftingRecipes" && !serializedObject.FindProperty("usesCrafting").boolValue)
            {
                Debug.Log("rank does not use crafting");
                continue;
            }
            EditorGUILayout.PropertyField(prop, true);
        }
        serializedObject.ApplyModifiedProperties();
        
    }
}
