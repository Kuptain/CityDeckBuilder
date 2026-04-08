using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

public class BuildingEditorWindow : ExtendedEditorWindow
{
    static BuildingDataContainer container;
    public static void Open(BuildingDataContainer _container)
    {
        BuildingEditorWindow window = GetWindow<BuildingEditorWindow>("Building editor");
        if (container == null)
        {
            container = _container;
            //CreateNewContainer();
        }
        window.serializedObject = new SerializedObject(container);
        //UpdateBuildings();
    }
    static void CreateNewContainer()
    {
        container = CreateInstance<BuildingDataContainer>();
        AssetDatabase.CreateAsset(container, "Assets/scriptableObjects/BuildingContainer.asset");
        UpdateBuildings();
    }
    static BuildingData[] GetAllBuildingData()
    {
        List<BuildingData> returnData = new List<BuildingData>();
        string path = "Assets/scriptableObjects/Buildings";
        DirectoryInfo info = new DirectoryInfo(path);
        FileInfo[] files = info.GetFiles("*.Asset", SearchOption.TopDirectoryOnly);
        
        // if not null, do something
        foreach (var file in files)
        {
            if (file != null)
            {
                var asset = AssetDatabase.LoadAssetAtPath(path+"/"+ file.Name,typeof(BuildingData));
                if(asset!=null)
                returnData.Add((BuildingData) asset);
            }
        }
        return returnData.ToArray();
    }

    static void UpdateBuildings()
    {
        container.buildings = GetAllBuildingData();
        EditorUtility.SetDirty(container);
        AssetDatabase.SaveAssets();

    }

    private void OnGUI()
    {

        if (container == null || container.buildings == null)
        {
            //CreateNewContainer();
        }
        if (serializedObject == null)
        {
            serializedObject = new SerializedObject(container);
        }
        if (serializedObject == null)
        {
            return;
        }
        serializedObject.UpdateIfRequiredOrScript();
        currentProperty = serializedObject.FindProperty("buildings");

        //starts to draw the fields
        EditorGUILayout.BeginHorizontal("box");
        /*
        if (GUILayout.Button("") && !EditorApplication.isPlaying)
        {
            PrefabUtility.ApplyPrefabInstance(data.gameObject, InteractionMode.UserAction);
        }
        */
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(150), GUILayout.ExpandHeight(true));

        DrawFullSidebar(currentProperty);

        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
        //zeichnet den jeweils ausgewählten sound
        if (selectedProperty != null)
        {
            DrawSelectedPropertiesPanel();
        }
        else
        {
            EditorGUILayout.LabelField("Select a Building you want to edit");
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();


        Apply();

    }

    void DrawSelectedPropertiesPanel()
    {
        EditorGUILayout.BeginVertical("box");
        currentProperty = selectedProperty;
        DrawField("buildingName", true);
        DrawField("buildingDescription", true);
        DrawField("prefab", true);
        DrawField("uiButton", true);
        DrawField("uiIcon", true);
        DrawField("ranks", true);
        EditorGUILayout.EndVertical();

    }
    void DrawFullSidebar(SerializedProperty prop)
    {
        base.DrawSidebar(prop);
        if (GUILayout.Button("Add Building", EditorStyles.toolbarButton))
        {
            container.AddBuilding();
        }

    }
}

