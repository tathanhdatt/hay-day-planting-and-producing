using UnityEngine;

public class ShopViewPresenter : BaseViewPresenter
{
    private readonly ICurrency currency;
    private readonly ILevelXpStorage levelXpStorage;
    private ShopView shopView;

    public ShopViewPresenter(GamePresenter gamePresenter, Transform transform, 
        ICurrency currency, ILevelXpStorage levelXpStorage) : base(gamePresenter, transform)
    {
        this.currency = currency;
        this.levelXpStorage = levelXpStorage;
    }

    protected override void AddViews()
    {
        this.shopView = AddView<ShopView>();
        Messenger.AddListener(Message.OutOfShopBound, OutOfShopBoundHandler);
        this.currency.OnAmountChanged += OnAmountChangedHandle;
        this.levelXpStorage.OnLevelUpdated += OnLevelUpdatedHandler;
    }

    private void OnLevelUpdatedHandler(int level)
    {
        this.shopView.OnUpdateLevel();
    }

    private void OnAmountChangedHandle(CurrencyType type)
    {
        UpdateEnoughCurrencyStatusOfShopItems(type);
    }

    private void UpdateEnoughCurrencyStatusOfShopItems(CurrencyType type)
    {
        ShopItemHolder[] itemHolders = this.shopView.ItemHolders;
        foreach (ShopItemHolder itemHolder in itemHolders)
        {
            foreach (ShopItemInfo itemInfo in itemHolder.ItemInfos)
            {
                if (itemInfo.currencyType != type) continue;
                itemInfo.isEnoughCurrency = this.currency.IsEnough(type, itemInfo.price);
            }
        }
    }

    private async void OutOfShopBoundHandler()
    {
        await Hide();
    }

    protected override void OnShow()
    {
        base.OnShow();
        this.shopView.OnClickExit += OnClickExitHandler;
        this.shopView.RefreshHolders();
    }

    protected override void OnHide()
    {
        base.OnHide();
        this.shopView.OnClickExit -= OnClickExitHandler;
    }

    private async void OnClickExitHandler()
    {
        await Hide();
    }
}