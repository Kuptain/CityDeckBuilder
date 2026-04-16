using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Callbacks;


[CustomPropertyDrawer(typeof(RankData))]
public class RankDataEditor : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var container = new VisualElement();
        SerializedProperty prop = property.FindPropertyRelative("housingIncrease");
        container.Add(new PropertyField(prop));
        SerializedProperty useCraftProp = property.FindPropertyRelative("usesCrafting");

        while (prop.NextVisible(false))
        {
            PropertyField field = new PropertyField(prop);
            container.Add(field);

            if (prop.name == "craftingRecipes")
            {
                if(!useCraftProp.boolValue)
                field.style.display = DisplayStyle.None;

                container.TrackPropertyValue(useCraftProp, _ =>
                {
                    var usesCrafting = useCraftProp.boolValue;
                    if (usesCrafting)
                    {
                        field.style.display = DisplayStyle.Flex;
                    }
                    else
                    {
                        field.style.display = DisplayStyle.None;
                    }
                });
                return container;
            }
           

        }
        return container;





    }
}
