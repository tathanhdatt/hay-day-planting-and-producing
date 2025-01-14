using UnityEngine;

public class BarnStorageViewPresenter : StorageViewPresenter
{
    public BarnStorageViewPresenter(GamePresenter gamePresenter, Transform transform,
        GoodsDatabase database, ILevelXpStorage levelXpStorage,
        ICurrency currency) : base(gamePresenter, transform, database, levelXpStorage, currency)
    {
    }

    protected override void AddStorageView()
    {
        this.storageView = AddView<BarnStorageView>();
    }

    protected override void AddUpgradeView()
    {
        this.upgradeView = AddView<BarnUpgradeStorageView>(false);
    }
}