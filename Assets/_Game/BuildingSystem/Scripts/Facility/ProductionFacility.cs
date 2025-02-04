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

    private TimeSpan producedTime;
    private TimeSpan passedTime;

    public override void Initialize(BuildingSystem buildingSystem, GridLayout gridLayout,
        TimerTooltip tooltip, ItemInfo info, FacilityData data = null)
    {
        base.Initialize(buildingSystem, gridLayout, tooltip, info, data);
        this.producedTimer.Initialize(false);
        this.producedTimer.OnFinished += OnProducedFinishedHandler;
        if (data != null)
        {
            LoadData(data as ProducibleFacilityData);
        }
    }

    private void OnProducedFinishedHandler()
    {
        this.producingIcons.RemoveFirst();
        this.productionTooltip.RefreshIfActive();
        SpawnProduct(this.currentRecipe);
        this.isProducing = false;
    }

    private void SpawnProduct(GoodsRecipe recipe)
    {
        CollectibleItem item =
            ServiceLocator.GetService<IPoolService>().Spawn(this.prefab, transform);
        int index = this.collectibles.Count % this.productSpawnPoints.Length;
        Vector3 pos = this.productSpawnPoints[index].position;
        item.SetPosition(pos);
        item.Show(recipe);
        this.collectibles.Add(item);
    }

    private void LoadData(ProducibleFacilityData data)
    {
        this.data = data;
        foreach (string productName in data.productNames)
        {
            foreach (GoodsRecipe recipe in this.recipes)
            {
                string productInRecipeName = recipe.product.goodsName;
                if (!productInRecipeName.Equals(productName)) continue;
                LoadRecipe(recipe, data);
            }
        }


        foreach (GoodsRecipe recipe in this.recipes)
        {
            string productInRecipeName = recipe.product.goodsName;
            foreach (string collectibleProductName in data.collectibleProductNames)
            {
                if (productInRecipeName != collectibleProductName) continue;
                SpawnProduct(recipe);
            }
        }
    }

    private void LoadRecipe(GoodsRecipe recipe, ProducibleFacilityData data)
    {
        if (this.currentRecipe != null)
        {
            OnAddRecipeHandler(recipe);
            return;
        }

        DateTime finishedTime = DateTime.Parse(data.finishedProducingTime);
        TimeSpan remainTime = finishedTime - DateTime.Now;
        OnAddRecipeHandler(recipe);
        Produce();
        this.passedTime = this.producedTime - remainTime;
        if (remainTime <= TimeSpan.Zero)
        {
            this.passedTime = DateTime.Now - finishedTime;
            this.producedTimer.SkipTimer();
        }
        else
        {
            this.passedTime = this.producedTime - remainTime;
            this.producedTimer.Subtract(this.passedTime);
        }
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

    private void OnAddRecipeHandler(GoodsRecipe recipe)
    {
        this.recipesQueue.Add(recipe);
        this.producingIcons.Add(recipe.growingGraphics.Last());
        this.productionTooltip.RefreshIfActive();
        if (this.isProducing) return;
        Produce();
    }

    private async void Produce()
    {
        while (this.recipesQueue.Count > 0)
        {
            this.isProducing = true;
            this.currentRecipe = this.recipesQueue.RemoveFirst();
            this.producedTime = new TimeSpan(
                this.currentRecipe.days,
                this.currentRecipe.hours,
                this.currentRecipe.minutes,
                this.currentRecipe.seconds);
            this.producedTimer.StartTimer(string.Empty, this.producedTime);
            if (this.passedTime > TimeSpan.Zero)
            {
                this.producedTimer.Subtract(this.passedTime);
                this.passedTime -= this.producedTime;
            }

            await UniTask.WaitUntil(() => !this.isProducing);
        }
        this.passedTime = TimeSpan.Zero;
    }

    private void OnProductionTooltipHiddenHandler()
    {
        this.productionTooltip.OnHidden -= OnProductionTooltipHiddenHandler;
        this.productionTooltip.OnAddRecipe -= OnAddRecipeHandler;
    }

    protected override void UpdateData()
    {
        this.data ??= new ProducibleFacilityData();
        base.UpdateData();
    }

    private ProducibleFacilityData GetProducibleData()
    {
        this.data ??= new ProducibleFacilityData();
        return this.data as ProducibleFacilityData;
    }

    public override FacilityData GetData()
    {
        ProducibleFacilityData producibleFacilityData = GetProducibleData();
        producibleFacilityData.productNames.Clear();
        producibleFacilityData.collectibleProductNames.Clear();
        SaveProducingItems(producibleFacilityData);
        SaveCollectibleItems(producibleFacilityData);
        return base.GetData();
    }

    private void SaveProducingItems(ProducibleFacilityData producibleFacilityData)
    {
        if (!this.isProducing) return;
        producibleFacilityData.productNames.Add(this.currentRecipe.product.goodsName);
        producibleFacilityData.finishedProducingTime = this.producedTimer.GetFinishTimeString();
        foreach (GoodsRecipe recipe in this.recipesQueue)
        {
            producibleFacilityData.productNames.Add(recipe.product.goodsName);
        }
    }

    private void SaveCollectibleItems(ProducibleFacilityData producibleFacilityData)
    {
        foreach (CollectibleItem item in this.collectibles)
        {
            producibleFacilityData.collectibleProductNames.Add(item.name);
        }
    }

    public void SetProduction(ProductionTooltip tooltip)
    {
        this.productionTooltip = tooltip;
    }

    public void SetDatabase(GoodsDatabase database)
    {
        this.barnDatabase = database;
    }
}