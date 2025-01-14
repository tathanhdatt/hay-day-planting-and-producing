using UnityEngine;

[CreateAssetMenu(fileName = "New Supply Requirement", menuName = "Goods/Upgrade/Goods Requirement")]
public class SupplyRequirement : ScriptableObject
{
    public int requiredQuantity;
    public Goods goods;

    public int GetGemToBuy()
    {
        int quantity = this.goods.quantity;
        int neededQuantity = this.requiredQuantity - quantity;
        return ExchangeRate.GoodsToGems * neededQuantity;
    }

    public void AddToEnoughQuantity()
    {
        this.goods.quantity += this.requiredQuantity - this.goods.quantity;
    }
}