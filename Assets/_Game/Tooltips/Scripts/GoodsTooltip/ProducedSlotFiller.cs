using Dt.Attribute;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ProducedSlotDetector))]
public class ProducedSlotFiller : MonoBehaviour
{
    [SerializeField, Required]
    private ProducedSlotDetector detector;

    [SerializeField, Required]
    private GoodsItem goodsItem;

    [SerializeField, ReadOnly]
    private GoodsRecipe recipe;

    public void Initialize()
    {
        this.detector.Initialize();
        this.detector.OnDetectedSlot += OnDetectedSlotHandler;
    }

    public void Show(GoodsRecipe recipe)
    {
        this.recipe = recipe;
        this.goodsItem.Initialize(this.recipe.product);
        Refresh();
    }

    private void OnDetectedSlotHandler(ProducedSlot slot)
    {
        slot.AddRecipe(this.recipe);
    }

    public void Refresh()
    {
        this.goodsItem.Refresh();
    }
}