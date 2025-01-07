using Cysharp.Threading.Tasks;
using Dt.Attribute;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [Title("Background")]
    [SerializeField, Required]
    private Image unlockedBackground;

    [SerializeField, Required]
    private Image lockedBackground;

    [Title("Info")]
    [SerializeField, Required]
    private Image icon;

    [SerializeField, Required]
    private TMP_Text title;

    [SerializeField, Required]
    private TMP_Text description;

    [SerializeField, Required]
    private TMP_Text unlockedLevelText;

    [SerializeField, ReadOnly]
    private CurrencyType currencyType;

    [SerializeField, ReadOnly]
    private int unlockedLevel;

    [Title("Currency")]
    [SerializeField, Required]
    private GameObject currencyGameObject;

    [SerializeField, Required]
    private Image currencyIcon;

    [SerializeField, Required]
    private TMP_Text price;

    [Title("")]
    [SerializeField, Required]
    private CurrencyGraphic[] currencyGraphics;

    [SerializeField, Required]
    private DraggedShopItem draggedShopItem;

    [SerializeField, ReadOnly]
    private ShopItemInfo info;

    public async UniTask Initialize(ShopItemInfo info, Transform draggingBound)
    {
        this.draggedShopItem.Initialize(draggingBound, this.icon.transform as RectTransform, info);
        this.info = info;
        DisplayInfo();
        UpdateStatus();
        Messenger.AddListener<int>(Message.UpdatedLevel, UpdatedLevelHandler);
        await UniTask.CompletedTask;
    }


    private void DisplayInfo()
    {
        this.icon.sprite = this.info.icon;
        this.title.SetText(this.info.title);
        this.description.SetText(this.info.description);
        this.currencyType = this.info.currencyType;
        this.price.SetText(this.info.price.ToString());
        this.unlockedLevel = this.info.unlockLevel;
        this.unlockedLevelText.SetText($"Unlock at level {this.info.unlockLevel}");
        SetCurrencyIcon();
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

    private void SetCurrencyIcon()
    {
        foreach (CurrencyGraphic graphic in this.currencyGraphics)
        {
            if (graphic.type == this.currencyType)
            {
                this.currencyIcon.sprite = graphic.icon;
            }
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
        this.unlockedBackground.gameObject.SetActive(true);
        this.currencyGameObject.SetActive(true);
        this.draggedShopItem.gameObject.SetActive(true);

        this.lockedBackground.gameObject.SetActive(false);
        this.unlockedLevelText.gameObject.SetActive(false);
    }

    private void SetLocked()
    {
        this.unlockedBackground.gameObject.SetActive(false);
        this.currencyGameObject.SetActive(false);
        this.draggedShopItem.gameObject.SetActive(false);

        this.lockedBackground.gameObject.SetActive(true);
        this.unlockedLevelText.gameObject.SetActive(true);
    }
}