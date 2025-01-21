using Core.Service;
using Dt.Attribute;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [SerializeField, Required]
    private PositionRandomizer positionRandomizer;

    [SerializeField, Required]
    private SpriteRenderer icon;

    [SerializeField, ReadOnly]
    private GoodsRecipe recipe;

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void Show(GoodsRecipe recipe)
    {
        this.recipe = recipe;
        this.icon.sprite = this.recipe.product.graphic;
        this.positionRandomizer.Randomize();
        gameObject.SetActive(true);
    }

    public void Collect()
    {
        Messenger.Broadcast(Message.CollectGoods,
            this.recipe.product, ExchangeRate.ProductionRate, transform.position);
        Messenger.Broadcast(Message.CollectRewards,
            this.recipe.rewards, transform.position);
        ServiceLocator.GetService<IPoolService>().Despawn(this);
    }
}