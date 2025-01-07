using System;
using System.Text;
using Cysharp.Threading.Tasks;
using Dt.Attribute;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameView : BaseView
{
    [Title("Level")]
    [SerializeField, Required]
    private FillBar levelXpBar;

    [SerializeField, Required]
    private TMP_Text levelText;

    [SerializeField, Required]
    private TMP_Text levelXpText;

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
        this.coinText.SetText(GetStringNumber(coin));
    }

    public void SetGemText(int gem)
    {
        this.gemText.SetText(GetStringNumber(gem));
    }

    public void FillLevelXpBar(float percentage)
    {
        this.levelXpBar.FillTo(percentage);
    }

    public void SetFillBar(float percentage)
    {
        this.levelXpBar.SetFillAmount(percentage);
    }

    public void SetLevel(int level)
    {
        this.levelText.SetText(GetStringNumber(level));
    }

    public void SetLevelXpText(int currentXp, int requiredXp)
    {
        string xpProgression = $"{currentXp}/{requiredXp}";
        this.levelXpText.SetText(xpProgression);
    }

    private string GetStringNumber(int number)
    {
        if (number == 0)
        {
            return "<sprite=\"0\" index=0>";
        }

        StringBuilder stringNumber = new StringBuilder();
        while (number > 0)
        {
            stringNumber.Insert(0, $"<sprite=\"{number % 10}\" index=0>");
            number /= 10;
        }

        return stringNumber.ToString();
    }
}