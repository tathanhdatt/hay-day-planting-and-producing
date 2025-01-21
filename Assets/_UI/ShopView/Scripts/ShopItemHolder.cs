using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Dt.Attribute;
using UnityEngine;

public class ShopItemHolder : MonoBehaviour
{
    [SerializeField, Required]
    private ShopItem shopItemPrefab;

    [SerializeField, Required]
    private ItemInfo[] shopItems;

    [SerializeField, Required]
    private Transform holder;

    [SerializeField]
    private ShopItemType itemType;

    public ShopItemType ItemType => this.itemType;
    public ItemInfo[] ItemInfos => this.shopItems;
    
    private readonly List<ShopItem> items = new List<ShopItem>();

    public async UniTask Initialize(Transform draggingBound)
    {
        await InstantiateItems(draggingBound);
    }

    private async UniTask InstantiateItems(Transform draggingBound)
    {
        await UniTask.CompletedTask;
        foreach (ItemInfo info in this.shopItems)
        {
            ShopItem newItem = Instantiate(this.shopItemPrefab, this.holder);
            this.items.Add(newItem);
            await newItem.Initialize(info, draggingBound);
        }
    }

    public void Refresh()
    {
        foreach (ShopItem item in this.items)
        {
            item.Refresh();
        }
    }

    public void OnUpdateLevel()
    {
        foreach (ItemInfo info in ItemInfos)
        {
            info.UpdateQuantity();
        }
        Refresh();
    }
}