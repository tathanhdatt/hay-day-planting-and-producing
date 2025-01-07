using Dt.Attribute;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Item", menuName = "Shop Item")]
public class ShopItemInfo : ScriptableObject
{
    public Sprite icon;
    public string title;
    public string description;
    public bool isUnlocked;
    public int unlockLevel;
    public CurrencyType currencyType;
    public int price;
    public PlaceableObject prefab;
    [ReadOnly]
    public bool isEnoughCurrency;
}