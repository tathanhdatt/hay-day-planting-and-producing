using System;
using System.Globalization;
using Cysharp.Threading.Tasks;
using Dt.Attribute;
using UnityEngine;

public class CropFacility : SingleSlotFacility
{
    [Title("Crop facility")]
    [SerializeField, Required]
    protected SpriteRenderer graphic;

    [SerializeField, Required]
    private Sprite defaultGraphic;

    [SerializeField, ReadOnly]
    private HarvestTooltip harvestTooltip;

    [SerializeField, ReadOnly]
    private bool canHarvest;

    public override void Initialize(BuildingSystem buildingSystem, GridLayout gridLayout,
        TimerTooltip tooltip, ItemInfo info, FacilityData data = null)
    {
        this.producedTimer.OnHeartbeat += OnHeartbeatHandler;
        base.Initialize(buildingSystem, gridLayout, tooltip, info, data);
        RegisterFreeSlotEvents();
    }

    private void RegisterFreeSlotEvents()
    {
        this.slot.OnFreeSlot += OnFreeSlotHandler;
    }

    private async void OnFreeSlotHandler()
    {
        this.graphic.sprite = this.currentRecipe.finishedGraphic;
        await UniTask.WaitForSeconds(0.6f);
        this.graphic.sprite = this.defaultGraphic;
        this.canHarvest = false;
        (this.data as ProducibleFacilityData)?.productNames.RemoveFirst();
    }

    private void OnHeartbeatHandler(int time)
    {
        Debug.Log(time);
        time = Math.Clamp(time, 0, this.currentRecipe.growingGraphics.Count);
        this.graphic.sprite = this.currentRecipe.growingGraphics[time - 1];
    }

    protected override void OnProducedFinishedHandler()
    {
        base.OnProducedFinishedHandler();
        this.canHarvest = true;
        this.graphic.sprite = this.currentRecipe.growingGraphics.Last();
    }

    protected override void ProduceCurrentRecipe()
    {
        // this.graphic.sprite = this.currentRecipe.growingGraphics[0];
        base.ProduceCurrentRecipe();
    }


    public void SetHarvestTooltip(HarvestTooltip tooltip)
    {
        this.harvestTooltip = tooltip;
    }

    protected override void ShowTooltips()
    {
        base.ShowTooltips();
        ShowProductionTimer();
    }

    private void ShowProductionTimer()
    {
        if (this.isProducing)
        {
            this.timerTooltip.Show(this.producedTimer);
        }
    }

    protected override void ShowGoodsTooltip()
    {
        if (this.canHarvest)
        {
            ShowHarvestTooltip();
        }
        else
        {
            base.ShowGoodsTooltip();
        }
    }

    private void ShowHarvestTooltip()
    {
        this.harvestTooltip.Show(transform);
    }
}