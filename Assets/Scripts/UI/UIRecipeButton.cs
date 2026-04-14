using UnityEngine;

public class UIRecipeButton : MonoBehaviour
{
    public CraftRecipe recipe;
    public void StartRecipe()
    {
        UI_HoverTooltip.Instance.StartRecipe(recipe);
    }
}
