using System.Collections.Generic;
using Dt.Attribute;
using UnityEngine;
using UnityEngine.EventSystems;

public class GoodsTooltip : MonoBehaviour
{
    [SerializeField, Required]
    private SnappingCamera snappingCamera;

    [SerializeField]
    private List<GoodsItem> recipeSlots;

    [SerializeField, ReadOnly]
    private List<GoodsRecipe> recipes;

    private void Awake()
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void Show(List<GoodsRecipe> recipes, Transform source)
    {
        this.snappingCamera.SetSource(source);
        this.recipes = recipes;
        FillSlots();
        gameObject.SetActive(true);
    }

    private void FillSlots()
    {
        int numberOfRecipes = this.recipes.Count;
        for (int i = 0; i < this.recipeSlots.Count; i++)
        {
            if (i < numberOfRecipes)
            {
                this.recipeSlots[i].Initialize(this.recipes[i].product);
                this.recipeSlots[i].gameObject.SetActive(true);
            }
            else
            {
                this.recipeSlots[i].gameObject.SetActive(false);
            }
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}