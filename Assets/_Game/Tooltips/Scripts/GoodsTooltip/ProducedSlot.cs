using System;
using Dt.Attribute;
using UnityEngine;

public class ProducedSlot : MonoBehaviour
{
    private const string notEnoughMaterialsMessage = "Not enough materials!";

    [SerializeField, ReadOnly]
    private bool isFull;

    [SerializeField, ReadOnly]
    private bool isProducing;

    [SerializeField, ReadOnly]
    private GoodsRecipe recipe;

    public GoodsRecipe Recipe => this.recipe;

    public event Action<GoodsRecipe> OnAddRecipe;
    public event Action OnFreeSlot;

    public void AddRecipe(GoodsRecipe recipe)
    {
        if (!enabled) return;
        if (this.isFull) return;
        this.recipe = null;
        if (IsEnoughMaterials(recipe))
        {
            this.isFull = true;
            OnAddRecipe?.Invoke(recipe);
            this.recipe = recipe;
        }
        else
        {
            Messenger.Broadcast(Message.PopupDialog, notEnoughMaterialsMessage);
        }
    }

    private bool IsEnoughMaterials(GoodsRecipe recipe)
    {
        foreach (GoodsRequirement requirement in recipe.materials)
        {
            if (requirement.requiredQuantity > requirement.goods.quantity)
            {
                return false;
            }
        }

        return true;
    }

    public void FreeSlot()
    {
        this.isFull = false;
        OnFreeSlot?.Invoke();
    }

    public bool CanFreeSlot()
    {
        if (!this.isFull) return false;
        if (this.isProducing) return false;
        return true;
    }

    public void SetIsProducing(bool isProducing)
    {
        this.isProducing = isProducing;
    }
}