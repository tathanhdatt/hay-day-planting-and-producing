using Cysharp.Threading.Tasks;
using Dt.Attribute;
using UnityEngine;

public class ShopItemHolder : MonoBehaviour
{
    [SerializeField, Required]
    private ShopItem shopItemPrefab;

    [SerializeField, Required]
    private ShopItemInfo[] shopItems;

    [SerializeField, Required]
    private Transform holder;

    [SerializeField]
    private ShopItemType itemType;

    public ShopItemType ItemType => this.itemType;
    public ShopItemInfo[] ItemInfos => this.shopItems;

    public async UniTask Initialize(Transform draggingBound)
    {
        await InstantiateItems(draggingBound);
    }

    private async UniTask InstantiateItems(Transform draggingBound)
    {
        await UniTask.CompletedTask;
        foreach (ShopItemInfo info in this.shopItems)
        {
            ShopItem newItem = Instantiate(this.shopItemPrefab, this.holder);
            await newItem.Initialize(info, draggingBound);
        }
    }
}