using System;
using Dt.Attribute;
using UnityEngine;

public class ProducedSlot : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private bool isFull;

    public event Action<GoodsRecipe> OnAddRecipe;
    public event Action OnFreeSlot;

    public void AddRecipe(GoodsRecipe recipe)
    {
        if (this.isFull) return;
        this.isFull = true;
        OnAddRecipe?.Invoke(recipe);
    }

    public void FreeSlot()
    {
        this.isFull = false;
        OnFreeSlot?.Invoke();
    }
}