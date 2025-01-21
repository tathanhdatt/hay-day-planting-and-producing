using System;
using System.Collections.Generic;
using Core.Service;
using Cysharp.Threading.Tasks;
using Dt.Attribute;
using UnityEngine;

public class ProductionFacility : Facility
{
    [Title("Production facility")]
    [SerializeField, Required]
    private Transform[] productSpawnPoints;

    [SerializeField]
    protected List<GoodsRecipe> recipes;

    [SerializeField, Required]
    protected Timer producedTimer;

    [Line]
    [SerializeField, Required]
    private CollectibleItem prefab;

    [Title("Tooltips")]
    [SerializeField, ReadOnly]
    private ProductionTooltip productionTooltip;

    [Line]
    [SerializeField, ReadOnly]
    private ItemInfo facilityInfo;

    [SerializeField, ReadOnly]
    protected GoodsRecipe currentRecipe;

    [SerializeField, ReadOnly]
    protected bool isProducing;

    [SerializeField, ReadOnly]
    private List<Sprite> producingIcons;

    [SerializeField, ReadOnly]
    private List<GoodsRecipe> recipesQueue;

    [SerializeField, ReadOnly]
    private List<CollectibleItem> collectibles;

    [SerializeField, ReadOnly]
    private GoodsDatabase barnDatabase;

    public override void Initialize(BuildingSystem buildingSystem,
        GridLayout gridLayout, TimerTooltip tooltip)
    {
        base.Initialize(buildingSystem, gridLayout, tooltip);
        this.producedTimer.Initialize(false);
        this.producedTimer.OnFinished += OnProducedFinishedHandler;
    }

    private void OnProducedFinishedHandler()
    {
        this.producingIcons.RemoveFirst();
        this.productionTooltip.Refresh();
        CollectibleItem item =
            ServiceLocator.GetService<IPoolService>().Spawn(this.prefab, transform);
        int index = this.collectibles.Count % this.productSpawnPoints.Length;
        Vector3 pos = this.productSpawnPoints[index].position;
        item.SetPosition(pos);
        item.Show(this.currentRecipe);
        this.collectibles.Add(item);
        this.isProducing = false;
    }

    protected override void OnFingerTapHandler()
    {
        if (TryCollectProduct()) return;
        base.OnFingerTapHandler();
        if (this.isBuilding) return;
        this.productionTooltip.OnHidden += OnProductionTooltipHiddenHandler;
        this.productionTooltip.OnAddRecipe += OnAddRecipeHandler;
        this.productionTooltip.Show(this.recipes, transform,
            this.producedTimer, this.producingIcons, this.facilityInfo);
    }

    private bool TryCollectProduct()
    {
        bool canCollectProduct = this.collectibles.Count > 0;
        if (!canCollectProduct) return false;
        bool isStorageFull = this.barnDatabase.GetAvailableSlots() <= 0;
        if (isStorageFull)
        {
            Messenger.Broadcast(Message.PopupDialog, "No space available.");
            return false;
        }

        this.collectibles.RemoveFirst().Collect();
        return true;
    }

    private async void OnAddRecipeHandler(GoodsRecipe recipe)
    {
        this.recipesQueue.Add(recipe);
        this.producingIcons.Add(recipe.growingGraphics.Last());
        this.productionTooltip.Refresh();
        if (this.isProducing) return;
        await Produce();
    }

    private async UniTask Produce()
    {
        while (this.recipesQueue.Count > 0)
        {
            this.isProducing = true;
            this.currentRecipe = this.recipesQueue.RemoveLast();
            TimeSpan produceTime = new TimeSpan(
                this.currentRecipe.days,
                this.currentRecipe.hours,
                this.currentRecipe.minutes,
                this.currentRecipe.seconds);
            this.producedTimer.StartTimer(string.Empty, produceTime);
            await UniTask.WaitUntil(() => !this.isProducing);
        }
    }

    public void SetProduction(ProductionTooltip tooltip)
    {
        this.productionTooltip = tooltip;
    }

    private void OnProductionTooltipHiddenHandler()
    {
        this.productionTooltip.OnHidden -= OnProductionTooltipHiddenHandler;
        this.productionTooltip.OnAddRecipe -= OnAddRecipeHandler;
    }

    public void SetInfo(ItemInfo info)
    {
        this.facilityInfo = info;
    }

    public void SetDatabase(GoodsDatabase database)
    {
        this.barnDatabase = database;
    }
}