using Dt.Attribute;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Item", menuName = "Shop Item")]
public class ShopItemInfo : ScriptableObject
{
    [Title("Information")]
    public Sprite icon;
    public string title;
    public string description;
    public bool isUnlocked;
    public int unlockLevel;
    public CurrencyType currencyType;
    public int price;
    [Title("Prefab")]
    public Facility prefab;
    [Title("Building Time")]
    public int days;
    public int hours;
    public int minutes;
    public int seconds;
    [ReadOnly]
    public bool isEnoughCurrency;
}