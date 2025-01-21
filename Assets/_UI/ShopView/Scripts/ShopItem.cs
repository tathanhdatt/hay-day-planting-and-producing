using System.Text;
using Cysharp.Threading.Tasks;
using Dt.Attribute;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [Title("Info")]
    [SerializeField, Required]
    private Image icon;

    [SerializeField, Required]
    private TMP_Text title;

    [SerializeField, Required]
    private TMP_Text description;

    [SerializeField, Required]
    private TMP_Text unlockedLevelText;

    [SerializeField, Required]
    private TMP_Text quantityText;

    [SerializeField, ReadOnly]
    private CurrencyType currencyType;

    [SerializeField, ReadOnly]
    private int unlockedLevel;

    [Title("Currency")]
    [SerializeField, Required]
    private TMP_Text price;

    [SerializeField, Required]
    private CurrencyGraphic[] currencyGraphics;

    [Line]
    [SerializeField, Required]
    private DraggedShopItem draggedShopItem;

    [SerializeField, Required]
    private Material grayScaleMaterial;

    [SerializeField, ReadOnly]
    private ItemInfo info;

    public async UniTask Initialize(ItemInfo info, Transform draggingBound)
    {
        this.draggedShopItem.Initialize(draggingBound, this.icon.transform as RectTransform, info);
        this.info = info;
        Refresh();
        Messenger.AddListener<int>(Message.UpdatedLevel, UpdatedLevelHandler);
        await UniTask.CompletedTask;
    }

    public void Refresh()
    {
        UpdateInfo();
        UpdateStatus();
    }

    private void UpdateInfo()
    {
        SetIcon();
        SetTitle();
        SetDescription();
        SetPrice();
        SetUnlockedLevel();
        SetQuantity();
    }


    private void SetIcon()
    {
        this.icon.sprite = this.info.icon;
    }

    private void SetTitle()
    {
        this.title.SetText(this.info.title);
    }

    private void SetDescription()
    {
        this.description.SetText(this.info.description);
    }

    private void SetPrice()
    {
        this.currencyType = this.info.currencyType;
        this.price.SetText(GetPriceText(this.info.price));
    }

    private void SetUnlockedLevel()
    {
        this.unlockedLevel = this.info.unlockLevel;
        this.unlockedLevelText.SetText($"Unlock at level {this.info.unlockLevel}");
    }

    private string GetPriceText(int price)
    {
        StringBuilder priceText = new StringBuilder(36);
        priceText.Append(price);
        priceText.Append(" ");
        if (this.currencyType == CurrencyType.Gem)
        {
            priceText.Append("<sprite=\"diamond\" index=0>");
        }
        else
        {
            priceText.Append("<sprite=\"coin\" index=0>");
        }

        return priceText.ToString();
    }

    private void UpdateStatus()
    {
        if (this.info.isUnlocked)
        {
            SetUnlocked();
        }
        else
        {
            SetLocked();
        }
    }

    private void UpdatedLevelHandler(int level)
    {
        if (level != this.unlockedLevel) return;
        SetUnlocked();
        this.info.isUnlocked = true;
    }

    private void SetUnlocked()
    {
        this.price.gameObject.SetActive(true);
        bool isAvailable = this.info.quantity < this.info.maxQuantity;
        if (isAvailable)
        {
            this.draggedShopItem.enabled = true;
            this.icon.material = null;
        }
        else
        {
            this.icon.material = this.grayScaleMaterial;
        }

        this.draggedShopItem.gameObject.SetActive(isAvailable);

        this.unlockedLevelText.gameObject.SetActive(false);
    }

    private void SetLocked()
    {
        this.price.gameObject.SetActive(false);
        this.draggedShopItem.gameObject.SetActive(false);
        this.icon.material = this.grayScaleMaterial;

        this.unlockedLevelText.gameObject.SetActive(true);
    }

    private void SetQuantity()
    {
        if (this.info.maxQuantity == 1)
        {
            this.quantityText.SetText(string.Empty);
        }

        {
            this.quantityText.SetText($"{this.info.quantity}/{this.info.maxQuantity}");
        }
    }
}