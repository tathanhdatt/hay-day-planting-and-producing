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

    public event Action<GoodsRecipe> OnAddRecipe;
    public event Action OnFreeSlot;

    public void AddRecipe(GoodsRecipe recipe)
    {
        if (!enabled) return;
        if (this.isFull) return;
        if (IsEnoughMaterials(recipe))
        {
            this.isFull = true;
            OnAddRecipe?.Invoke(recipe);
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
        if (!this.isFull) return;
        if (this.isProducing) return;
        this.isFull = false;
        OnFreeSlot?.Invoke();
    }

    public void SetIsProducing(bool isProducing)
    {
        this.isProducing = isProducing;
    }
}