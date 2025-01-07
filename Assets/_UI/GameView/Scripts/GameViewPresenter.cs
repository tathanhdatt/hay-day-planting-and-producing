using System;
using UnityEngine;

public class GameViewPresenter : BaseViewPresenter
{
    private GameView gameView;
    private readonly ICurrency currency;
    private readonly ILevelXpStorage levelXpStorage;

    public GameViewPresenter(GamePresenter gamePresenter,
        Transform transform, ICurrency currency, ILevelXpStorage levelXpStorage)
        : base(gamePresenter, transform)
    {
        this.currency = currency;
        this.levelXpStorage = levelXpStorage;
    }

    protected override void AddViews()
    {
        this.gameView = AddView<GameView>();
    }

    protected override void OnShow()
    {
        base.OnShow();
        this.currency.OnAmountChanged += OnAmountChangedHandler;
        this.gameView.OnClickShop += OnClickShopHandler;
        this.gameView.OnClickAddCoin += OnClickAddCoinHandler;
        this.gameView.OnClickAddGem += OnClickAddGemHandler;
        this.gameView.OnClickAddXp += OnClickAddXpHandler;
        this.levelXpStorage.OnLevelUpdated += OnLevelUpdatedHandler;
        this.levelXpStorage.OnXpUpdated += OnXpUpdatedHandler;
        this.gameView.SetLevel(this.levelXpStorage.GetCurrentLevel());
        SetXpText(this.levelXpStorage.GetCurrentXp());
        UpdateXpBar();
    }


    protected override void OnHide()
    {
        base.OnHide();
        this.currency.OnAmountChanged -= OnAmountChangedHandler;
        this.gameView.OnClickShop -= OnClickShopHandler;
        this.gameView.OnClickAddCoin -= OnClickAddCoinHandler;
        this.gameView.OnClickAddGem -= OnClickAddGemHandler;
        this.gameView.OnClickAddXp -= OnClickAddXpHandler;
        this.levelXpStorage.OnLevelUpdated -= OnLevelUpdatedHandler;
        this.levelXpStorage.OnXpUpdated -= OnXpUpdatedHandler;
    }

    private async void OnClickShopHandler()
    {
        await GamePresenter.GetViewPresenter<ShopViewPresenter>().Show();
    }

    private void OnAmountChangedHandler(CurrencyType currencyType)
    {
        switch (currencyType)
        {
            case CurrencyType.Coin:
                this.gameView.SetCoinText(this.currency.GetAmount(currencyType));
                break;
            case CurrencyType.Gem:
                this.gameView.SetGemText(this.currency.GetAmount(currencyType));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(currencyType), currencyType, null);
        }
    }

    private void OnClickAddGemHandler()
    {
        this.currency.AddAmount(CurrencyType.Gem, 10);
    }

    private void OnClickAddCoinHandler()
    {
        this.currency.AddAmount(CurrencyType.Coin, 10);
    }

    private void OnClickAddXpHandler()
    {
        this.levelXpStorage.AddXp(10);
    }

    private void OnLevelUpdatedHandler(int level)
    {
        this.gameView.SetLevel(level);
        this.gameView.SetFillBar(0);
        SetXpText(0);
    }

    private void OnXpUpdatedHandler(int xp)
    {
        SetXpText(xp);
        UpdateXpBar();
    }

    private void SetXpText(int xp)
    {
        int requiredXp = this.levelXpStorage.GetRequiredXp();
        this.gameView.SetLevelXpText(xp, requiredXp);
    }

    private void UpdateXpBar()
    {
        this.gameView.FillLevelXpBar(this.levelXpStorage.GetXpPercent());
    }
}