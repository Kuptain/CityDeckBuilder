using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Callbacks;


[CustomEditor(typeof(BuildingData))]
public class BuildingDataEditor : Editor
{
    public VisualTreeAsset m_InspectorUXML;
    public override VisualElement CreateInspectorGUI()
    {
       
        var container = new VisualElement();
        if (m_InspectorUXML != null)
        {
            VisualElement uxmlContent = m_InspectorUXML.CloneTree();
            container.Add(uxmlContent);
        }
        SerializedProperty prop = serializedObject.FindProperty("buildingDescription");
        
        while (prop.NextVisible(false))
        {
            container.Add(new PropertyField(prop));
        }
        return container;
       
    }
}
