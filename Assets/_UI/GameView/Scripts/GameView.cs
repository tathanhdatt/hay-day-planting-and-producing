using System;
using Cysharp.Threading.Tasks;
using Dt.Attribute;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameView : BaseView
{
    [Title("Level")]

    [SerializeField, Required]
    private TMP_Text levelText;

    [SerializeField, Required]
    private TMP_Text levelXpText;

    [SerializeField, Required]
    private DeltaSizeFillBar levelXpFillBar;
    
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
    }

    public void SetCoinText(int coin)
    {
        this.coinText.SetText(coin.ToString());
    }

    public void SetGemText(int gem)
    {
        this.gemText.SetText(gem.ToString());
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

    public void SetFillBar(int percentage)
    {
        this.levelXpFillBar.SetFillBar(percentage);
    }

    public void FillLevelXpBar(float percentage)
    {
        this.levelXpFillBar.FillBar(percentage);
    }
}