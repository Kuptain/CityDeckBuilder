using UnityEngine;
using UnityEngine.UIElements;


public class ResourceConverterGroup
{
    public static ResourceDatabase database;
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadMethod]
#endif
    [RuntimeInitializeOnLoadMethod]
    public static void Register()
    {
        if (database == null)
        {
            database = (ResourceDatabase) Resources.Load("ResourceDatabase");
        }
        var group = new ConverterGroup("ResourceConverter");

        group.AddConverter((ref ResourceType type) => new StyleBackground(database.GetIcon(type)));

        ConverterGroups.RegisterConverterGroup(group);
    }
}
