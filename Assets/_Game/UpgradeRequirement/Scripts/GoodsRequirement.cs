using System;

[Serializable]
public class GoodsRequirement 
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