using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Dt.Attribute;
using UnityEngine;

public abstract class SingleSlotFacility : Facility
{
    [Title("Goods facility")]
    [SerializeField, ReadOnly]
    protected List<GoodsRecipe> productionQueue = new List<GoodsRecipe>(10);

    [SerializeField]
    protected List<GoodsRecipe> recipes;

    [SerializeField, ReadOnly]
    protected GoodsRecipe currentRecipe;

    [SerializeField]
    protected ProducedSlot slot;

    [SerializeField, Required]
    protected Timer producedTimer;

    [Title("Goods Tooltip")]
    [SerializeField, ReadOnly]
    protected CropTooltip cropTooltip;

    [SerializeField]
    private bool hideGoodsTooltipOnDrag;

    [SerializeField, ReadOnly]
    protected bool isProducing;

    private TimeSpan producedTime;

    public override void Initialize(BuildingSystem buildingSystem, GridLayout gridLayout,
        TimerTooltip tooltip, ItemInfo info, FacilityData data = null)
    {
        base.Initialize(buildingSystem, gridLayout, tooltip, info, data);
        InitializeSlot();
        this.producedTimer.Initialize(false);
        this.producedTimer.OnFinished += OnProducedFinishedHandler;
        if (data != null)
        {
            LoadData(data as ProducibleFacilityData);
        }
    }


    protected virtual void OnProducedFinishedHandler()
    {
        this.isProducing = false;
        this.slot.SetIsProducing(false);
        this.timerTooltip.Hide();
    }

    private void InitializeSlot()
    {
        this.slot.OnAddRecipe += OnAddRecipeHandler;
    }

    private void OnAddRecipeHandler(GoodsRecipe recipe)
    {
        if (this.hideGoodsTooltipOnDrag)
        {
            this.cropTooltip.HideContent();
        }

        this.productionQueue.Add(recipe);
        if (this.isProducing) return;
        Produce();
    }

    private async void Produce()
    {
        while (this.productionQueue.Count > 0)
        {
            this.currentRecipe = this.productionQueue.RemoveLast();
            this.currentRecipe.ConsumeMaterials();
            this.cropTooltip.RefreshContent();
            ProduceCurrentRecipe();
            await UniTask.WaitUntil(() => !this.isProducing);
        }

        this.isProducing = false;
    }

    private void ProduceCurrentRecipe()
    {
        this.slot.SetIsProducing(true);
        this.producedTime = CreateProducedTime();
        int heartbeat = this.currentRecipe.growingGraphics.Count - 1;
        this.isProducing = true;
        this.producedTimer.StartTimer(string.Empty, this.producedTime, heartbeat);
    }

    private TimeSpan CreateProducedTime()
    {
        return new TimeSpan(
            this.currentRecipe.days,
            this.currentRecipe.hours,
            this.currentRecipe.minutes,
            this.currentRecipe.seconds);
    }

    private void LoadData(ProducibleFacilityData data)
    {
        this.data = data;
        if (data.productNames.Count <= 0) return;
        string productName = data.productNames.RemoveFirst();
        foreach (GoodsRecipe recipe in this.recipes)
        {
            if (recipe.product.goodsName != productName) continue;
            this.slot.SetIsFull(true);
            this.slot.Recipe = recipe;
            DateTime finishedTime = DateTime.Parse(data.finishedProducingTime);
            TimeSpan remainTime = finishedTime - DateTime.Now;
            this.currentRecipe = recipe;
            ProduceCurrentRecipe();
            if (remainTime <= TimeSpan.Zero)
            {
                this.producedTimer.SkipTimer();
            }
            else
            {
                TimeSpan passedTime = this.producedTime - remainTime;
                this.producedTimer.Subtract(passedTime);
            }

            break;
        }
    }

    public void SetCropTooltip(CropTooltip tooltip)
    {
        this.cropTooltip = tooltip;
    }

    protected override void ShowTooltips()
    {
        base.ShowTooltips();
        ShowGoodsTooltip();
    }

    protected virtual void ShowGoodsTooltip()
    {
        if (this.isBuilding) return;
        if (this.isProducing) return;
        this.cropTooltip.Show(this.recipes, transform);
    }

    protected override void UpdateData()
    {
        this.data ??= new ProducibleFacilityData();
        base.UpdateData();
    }

    public override FacilityData GetData()
    {
        SaveCurrentProductData();
        return base.GetData();
    }

    private void SaveCurrentProductData()
    {
        ProducibleFacilityData producibleData = GetProducibleData();
        producibleData.productNames.Clear();
        if (this.currentRecipe == null) return;
        producibleData.finishedProducingTime = this.producedTimer.GetFinishTimeString();
        producibleData.productNames.Add(this.currentRecipe.product.name);
    }
    
    private ProducibleFacilityData GetProducibleData()
    {
        this.data ??= new ProducibleFacilityData();
        return this.data as ProducibleFacilityData;
    }
}