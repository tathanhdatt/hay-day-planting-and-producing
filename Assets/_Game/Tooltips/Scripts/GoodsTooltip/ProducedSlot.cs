using System;
using Dt.Attribute;
using UnityEngine;

public class ProducedSlot : MonoBehaviour
{
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
        this.isFull = true;
        OnAddRecipe?.Invoke(recipe);
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