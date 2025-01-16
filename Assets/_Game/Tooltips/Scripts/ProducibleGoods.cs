using Dt.Attribute;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ProducibleGoods : MonoBehaviour
{
    [SerializeField, Required]
    private GoodsItem goodsItem;

    [SerializeField, Required]
    private UIDrag drag;
    
    [SerializeField, Required]
    private Collider2D detectSlotCollider;

    [SerializeField, ReadOnly]
    private GoodsRecipe recipe;

    [SerializeField, ReadOnly]
    private ProducedSlot lastProducedSlot;

    [SerializeField, ReadOnly]
    private Collider2D lastCollider2D;

    public void Initialize()
    {
        this.detectSlotCollider.enabled = false;
        this.drag.OnFingerDown += OnFingerDownHandler;
        this.drag.OnFingerUp += OnFingerUpHandler;
    }

    private void OnFingerDownHandler()
    {
        this.detectSlotCollider.enabled = true;
    }

    private void OnFingerUpHandler()
    {
        this.detectSlotCollider.enabled = false;
    }

    public void Show(GoodsRecipe recipe)
    {
        this.recipe = recipe;
        this.goodsItem.Initialize(recipe.product);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        bool isNewCollider = this.lastCollider2D != other;
        if (isNewCollider)
        {
            ProducedSlot producedSlot = other.GetComponent<ProducedSlot>();
            if (producedSlot == null) return;
            this.lastProducedSlot = producedSlot;
        }

        this.lastProducedSlot.AddRecipe(this.recipe);
    }
}