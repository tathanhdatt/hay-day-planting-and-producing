using System;
using Dt.Attribute;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeStorageItem : MonoBehaviour
{
    [SerializeField, Required]
    private Image icon;

    [SerializeField, Required]
    private TMP_Text quantityText;

    [SerializeField, Required]
    private TMP_Text priceText;

    [SerializeField, Required]
    private TwiceClickButton buyButton;

    [SerializeField, Required]
    private Image enoughIcon;

    [SerializeField, ReadOnly]
    private GoodsRequirement requirement;

    public event Action<GoodsRequirement> OnConfirmBuyGoods;

    public void Initialize(GoodsRequirement requirement)
    {
        this.requirement = requirement;
        this.buyButton.OnConfirm += OnConfirmHandler;
        Refresh();
    }

    private void OnConfirmHandler()
    {
        OnConfirmBuyGoods?.Invoke(this.requirement);
    }

    public void Refresh()
    {
        SetIcon();
        SetQuantity();
        if (IsEnoughQuantity())
        {
            HandleEnoughQuantity();
        }
        else
        {
            HandleNotEnoughQuantity();
        }
    }

    private void SetIcon()
    {
        this.icon.sprite = this.requirement.goods.graphic;
    }

    private bool IsEnoughQuantity()
    {
        return this.requirement.goods.quantity >= this.requirement.requiredQuantity;
    }

    private void HandleEnoughQuantity()
    {
        this.enoughIcon.gameObject.SetActive(true);
        this.buyButton.gameObject.SetActive(false);
        this.priceText.gameObject.SetActive(false);
    }

    private void HandleNotEnoughQuantity()
    {
        this.enoughIcon.gameObject.SetActive(false);
        this.buyButton.gameObject.SetActive(true);
        this.priceText.gameObject.SetActive(true);
        SetBuyPrice();
    }

    private void SetQuantity()
    {
        int quantity = this.requirement.goods.quantity;
        string quantityPerRequirement = $"{quantity}/{this.requirement.requiredQuantity}";
        this.quantityText.SetText(quantityPerRequirement);
    }

    private void SetBuyPrice()
    {
        int price = this.requirement.GetGemToBuy();
        this.priceText.SetText($"{price}<sprite=\"diamond\" index=0>");
    }
}