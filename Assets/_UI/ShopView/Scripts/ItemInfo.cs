using Dt.Attribute;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Item", menuName = "Shop Item")]
public class ItemInfo : ScriptableObject
{
    [Title("Information")]
    public Sprite icon;
    public string title;
    public string description;
    public bool isUnlocked;
    public int unlockLevel;
    public CurrencyType currencyType;
    public int price;
    [Line]
    public int numberOfSlot;

    [Title("Quantity")]
    public int additionalQuantityEachLevel;
    public int quantity;
    public int maxQuantity;

    [Title("Prefab")]
    public Facility prefab;

    [Title("Building Time")]
    public int days;
    public int hours;
    public int minutes;
    public int seconds;

    [ReadOnly]
    public bool isEnoughCurrency;


    [Button]
    public void UpdateQuantity()
    {
        this.maxQuantity += this.additionalQuantityEachLevel;
    }

    [Button]
    public void IncreaseQuantity()
    {
        this.quantity += 1;
    }
}