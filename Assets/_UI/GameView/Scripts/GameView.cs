using System;
using System.Text;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Dt.Attribute;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameView : BaseView
{
    [Title("Level")]
    [SerializeField, Required]
    private Image levelXpBar;

    [SerializeField, Required]
    private TMP_Text levelText;

    [SerializeField, Required]
    private TMP_Text levelXpText;

    [SerializeField]
    private float xpBarUpdateDuration;

    [SerializeField, ReadOnly]
    private float maxXpBarWidth;
    
    private Tweener xpBarTweener;

    [Title("Currency")]
    [SerializeField, Required]
    private TMP_Text coinText;

    [SerializeField, Required]
    private TMP_Text gemText;

    [Title("Buttons")]
    [SerializeField, Required]
    private Button settingsButton;

    [SerializeField, Required]
    private Button shopButton;

    [SerializeField, Required]
    private Button friendsButton;

    [Title("Cheating")]
    [SerializeField]
    private int coinAmount;

    [SerializeField]
    private Button addCoinButton;

    [SerializeField]
    private Button addGemButton;

    [SerializeField]
    private Button addXpButton;

    public event Action OnClickShop;
    public event Action OnClickAddCoin;
    public event Action OnClickAddGem;
    public event Action OnClickAddXp;

    public override async UniTask Initialize()
    {
        await base.Initialize();
        this.shopButton.onClick.AddListener(() => OnClickShop?.Invoke());
        this.addCoinButton.onClick.AddListener(() => OnClickAddCoin?.Invoke());
        this.addGemButton.onClick.AddListener(() => OnClickAddGem?.Invoke());
        this.addXpButton.onClick.AddListener(() => OnClickAddXp?.Invoke());
        this.maxXpBarWidth = ((RectTransform)this.levelXpBar.transform.parent.transform).rect.width;
    }

    public void SetCoinText(int coin)
    {
        this.coinText.SetText(coin.ToString());
    }

    public void SetGemText(int gem)
    {
        this.gemText.SetText(gem.ToString());
    }

    public async void FillLevelXpBar(float percentage)
    {
        this.xpBarTweener?.Complete();
        Vector2 sizeDelta = GetLevelXpBarSize();
        sizeDelta.x = percentage * this.maxXpBarWidth;
        this.xpBarTweener = this.levelXpBar.rectTransform
            .DOSizeDelta(sizeDelta, this.xpBarUpdateDuration);
        this.xpBarTweener.SetEase(Ease.OutQuart);
        await this.xpBarTweener.AsyncWaitForCompletion();
    }

    public void SetFillBar(float percentage)
    {
        this.xpBarTweener?.Complete();
        Vector2 sizeDelta = GetLevelXpBarSize();
        sizeDelta.x = percentage * this.maxXpBarWidth;
        this.levelXpBar.rectTransform.sizeDelta = sizeDelta;
    }

    private Vector2 GetLevelXpBarSize()
    {
        return this.levelXpBar.rectTransform.sizeDelta;
    }

    public void SetLevel(int level)
    {
        this.levelText.SetText(level.ToString());
    }

    public void SetLevelXpText(int currentXp, int requiredXp)
    {
        string xpProgression = $"{currentXp}/{requiredXp}";
        this.levelXpText.SetText(xpProgression);
    }
}