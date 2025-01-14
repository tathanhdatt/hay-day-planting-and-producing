using System.Collections.Generic;
using Dt.Attribute;
using UnityEngine;

public class GoodsHolder : MonoBehaviour
{
    [SerializeField, Required]
    private Transform content;

    [SerializeField, Required]
    private GoodsItem prefab;

    [SerializeField, ReadOnly]
    private GoodsDatabase database;

    private readonly List<GoodsItem> items = new List<GoodsItem>(50);

    public void Initialize(GoodsDatabase database)
    {
        this.database = database;
        GenerateGoodsItems();
    }

    private void GenerateGoodsItems()
    {
        foreach (Goods goods in this.database.goods)
        {
            if (!goods.isUnlocked) continue;
            GoodsItem goodsItem = Instantiate(this.prefab, this.content);
            goodsItem.Initialize(goods);
            this.items.Add(goodsItem);
        }
    }

    public void Refresh()
    {
        foreach (GoodsItem item in this.items)
        {
            item.Refresh();
        }
    }
}