using UnityEngine;
using UnityEngine.UIElements;

public class KnowledgeConvertergroup
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
            database = (ResourceDatabase)Resources.Load("ResourceDatabase");
        }
        var group = new ConverterGroup("KnowledgeConverter");

        group.AddConverter((ref KnowledgeType type) => new StyleBackground(database.GetKnowledgeIcon(type)));

        ConverterGroups.RegisterConverterGroup(group);
    }
}
