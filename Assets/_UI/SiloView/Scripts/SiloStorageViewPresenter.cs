using UnityEngine;

public class SiloStorageViewPresenter : StorageViewPresenter
{
    public SiloStorageViewPresenter(GamePresenter gamePresenter, Transform transform,
        GoodsDatabase database, ILevelXpStorage levelXpStorage,
        ICurrency currency) : base(gamePresenter, transform, database, levelXpStorage, currency)
    {
    }

    protected override void AddStorageView()
    {
        this.storageView = AddView<SiloStorageView>();
    }

    protected override void AddUpgradeView()
    {
        this.upgradeView = AddView<SiloUpgradeStorageView>(false);
    }
}