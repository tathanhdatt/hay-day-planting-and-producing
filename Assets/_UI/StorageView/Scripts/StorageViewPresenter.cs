using System;
using UnityEngine;

public abstract class StorageViewPresenter : BaseViewPresenter
{
    private readonly GoodsDatabase database;
    private readonly ILevelXpStorage levelXpStorage;
    private readonly ICurrency currency;
    protected StorageView storageView;
    protected UpgradeStorageView upgradeView;

    protected StorageViewPresenter(GamePresenter gamePresenter, Transform transform,
        GoodsDatabase database, ILevelXpStorage levelXpStorage, ICurrency currency
    ) : base(gamePresenter, transform)
    {
        this.database = database;
        this.levelXpStorage = levelXpStorage;
        this.currency = currency;
    }

    protected override void AddViews()
    {
        AddStorageView();
        AddUpgradeView();
        this.storageView.InitializeGoodsHolder(this.database);
        this.levelXpStorage.OnLevelUpdated += OnLevelUpdatedHandler;
        GenerateUpgradeItems();
    }

    protected abstract void AddStorageView();
    protected abstract void AddUpgradeView();

    private void OnLevelUpdatedHandler(int level)
    {
        foreach (Goods goods in this.database.goods)
        {
            if (goods.level == level)
            {
                goods.isUnlocked = true;
            }
        }
    }

    private void GenerateUpgradeItems()
    {
        UpgradeInformation upgradeInfo = this.database.GetCurrentUpgradeInformation();
        if (upgradeInfo == null)
        {
            throw new ArgumentException(
                $"There is no upgrade information for {this.database.capacity} capacity.");
        }

        foreach (SupplyRequirement requirement in upgradeInfo.requirements)
        {
            this.upgradeView.GenerateSupply(requirement);
        }
    }

    protected override void OnShow()
    {
        base.OnShow();
        this.storageView.OnClickExit += OnClickExitHandler;
        this.storageView.OnClickUpgrade += OnClickUpgradeHandler;
        this.upgradeView.OnClickBack += OnClickBackHandler;
        this.upgradeView.OnConfirmBuyGoods += OnConfirmBuyGoodsHandler;
        this.upgradeView.OnConfirmUpgrade += OnConfirmUpgradeHandler;
        this.storageView.SetActiveStorage(true);
        Refresh();
    }

    protected override void OnHide()
    {
        base.OnHide();
        this.storageView.OnClickExit -= OnClickExitHandler;
        this.storageView.OnClickUpgrade -= OnClickUpgradeHandler;
        this.upgradeView.OnClickBack -= OnClickBackHandler;
        this.upgradeView.OnConfirmBuyGoods -= OnConfirmBuyGoodsHandler;
        this.upgradeView.OnConfirmUpgrade -= OnConfirmUpgradeHandler;
    }

    private async void OnClickExitHandler()
    {
        await Hide();
    }

    private async void OnClickUpgradeHandler()
    {
        this.storageView.SetActiveStorage(false);
        await this.upgradeView.Show();
    }

    private async void OnClickBackHandler()
    {
        await this.upgradeView.Hide();
        this.storageView.SetActiveStorage(true);
    }

    private void OnConfirmBuyGoodsHandler(SupplyRequirement requirement)
    {
        int gem = requirement.GetGemToBuy();
        if (this.currency.IsEnough(CurrencyType.Gem, gem))
        {
            this.currency.SubtractAmount(CurrencyType.Gem, gem);
            requirement.AddToEnoughQuantity();
            Refresh();
        }
        else
        {
            // TODO: Notify not enough money
            Debug.Log($"Not enough gem to buy {requirement.goods.goodsName}");
        }
    }

    private void Refresh()
    {
        this.upgradeView.Refresh();
        this.storageView.RefreshHolder();
        UpdateCapacity();
        UpdateIndicator();
    }

    private void OnConfirmUpgradeHandler()
    {
        UpgradeBarnStorage();
    }

    private void UpgradeBarnStorage()
    {
        UpgradeInformation upgradeInfo = this.database.GetCurrentUpgradeInformation();
        if (upgradeInfo.CanUpgrade())
        {
            ConsumeGoodsToUpgrade(upgradeInfo);
            IncreaseRequiredQuantity(upgradeInfo);
            IncreaseStorageCapacity(upgradeInfo);
            Refresh();
        }
        else
        {
            // TODO: Notify not enough supply to upgrade
            Debug.Log("Not enough quantity to upgrade barn");
        }
    }

    private void ConsumeGoodsToUpgrade(UpgradeInformation upgradeInfo)
    {
        foreach (SupplyRequirement requirement in upgradeInfo.requirements)
        {
            requirement.goods.quantity -= requirement.requiredQuantity;
        }
    }

    private void IncreaseRequiredQuantity(UpgradeInformation upgradeInfo)
    {
        foreach (SupplyRequirement requirement in upgradeInfo.requirements)
        {
            requirement.requiredQuantity += upgradeInfo.additionalSupplyEachLevel;
        }
    }

    private void IncreaseStorageCapacity(UpgradeInformation upgradeInfo)
    {
        this.database.capacity += upgradeInfo.additionalCapacityEachLevel;
    }

    private void UpdateIndicator()
    {
        float storagePercentage = this.database.GetOccupiedSlots() / (float)this.database.capacity;
        this.storageView.MoveIndicator(storagePercentage);
    }

    private void UpdateCapacity()
    {
        int occupiedSlots = this.database.GetOccupiedSlots();
        this.storageView.SetCapacityText($"{occupiedSlots}/{this.database.capacity}");
    }
}