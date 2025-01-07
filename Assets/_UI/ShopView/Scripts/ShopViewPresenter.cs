using UnityEngine;

public class ShopViewPresenter : BaseViewPresenter
{
    private readonly ICurrency currency;
    private ShopView shopView;

    public ShopViewPresenter(GamePresenter gamePresenter, Transform transform, ICurrency currency)
        : base(gamePresenter, transform)
    {
        this.currency = currency;
    }

    protected override void AddViews()
    {
        this.shopView = AddView<ShopView>();
        Messenger.AddListener(Message.OutOfShopBound, OutOfShopBoundHandler);
        this.currency.OnAmountChanged += OnAmountChangedHandle;
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
                itemInfo.isEnoughCurrency = this.currency.InEnough(type, itemInfo.price);
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