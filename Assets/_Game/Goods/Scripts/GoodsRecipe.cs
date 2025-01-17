using System.Collections.Generic;
using Dt.Attribute;
using UnityEngine;

[CreateAssetMenu(fileName = "New Goods Recipe", menuName = "Goods/Goods Recipe")]
public class GoodsRecipe : ScriptableObject
{
    public Goods product;
    public List<GoodsRequirement> materials;
    [Title("Produce Time")]
    public int days;
    public int hours;
    public int minutes;
    public int seconds;
    [Title("Growing Graphic")]
    public List<Sprite> growingGraphics;
    public Sprite finishedGraphic;

    public void ConsumeMaterials()
    {
        foreach (GoodsRequirement requirement in this.materials)
        {
            requirement.goods.quantity -= requirement.requiredQuantity;
        }
    }
}