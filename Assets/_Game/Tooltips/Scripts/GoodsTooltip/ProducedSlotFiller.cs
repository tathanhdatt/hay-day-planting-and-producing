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
    private Image graphic;

    [SerializeField, Required]
    private TMP_Text quantity;

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
        this.graphic.sprite = recipe.product.graphic;
        this.quantity.SetText(this.recipe.product.quantity.ToString());
    }

    private void OnDetectedSlotHandler(ProducedSlot slot)
    {
        slot.AddRecipe(this.recipe);
    }
}