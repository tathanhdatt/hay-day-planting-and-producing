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
            UpdateProduceData();
            await UniTask.WaitUntil(() => !this.isProducing);
        }

        this.isProducing = false;
    }

    protected virtual void ProduceCurrentRecipe()
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
            this.producedTimer.Subtract(this.producedTime - remainTime);
            break;
        }

        UpdateProduceData();
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

    private void UpdateProduceData()
    {
        ProducibleFacilityData producibleFacilityData = GetProducibleData();
        if (this.isProducing)
        {
            SaveCurrentProductData();
        }

        Debug.Log("Data Updated");
        Debug.Log(JsonUtility.ToJson(producibleFacilityData, true));
        this.data = producibleFacilityData;
    }

    private ProducibleFacilityData GetProducibleData()
    {
        if (this.data is not ProducibleFacilityData producibleFacilityData)
        {
            producibleFacilityData = new ProducibleFacilityData(this.data);
        }

        return producibleFacilityData;
    }

    protected void SaveCurrentProductData()
    {
        ProducibleFacilityData producibleData = GetProducibleData();
        producibleData.finishedProducingTime =
            this.producedTimer.GetFinishTimeString();
        producibleData.productNames.Add(this.currentRecipe.product.name);
    }

    protected override void UpdateData()
    {
        this.data ??= new ProducibleFacilityData();
        base.UpdateData();
    }
}