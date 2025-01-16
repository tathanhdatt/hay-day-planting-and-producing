using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Dt.Attribute;
using UnityEngine;

public class ProductionFacility : Facility
{
    private readonly Queue<GoodsRecipe> productionQueue = new Queue<GoodsRecipe>(10);

    [Title("Production facility")]
    [SerializeField, Required]
    private SpriteRenderer graphic;

    [SerializeField]
    private List<GoodsRecipe> recipes;

    [SerializeField, ReadOnly]
    private GoodsRecipe currentRecipe;

    [SerializeField]
    private List<ProducedSlot> slots;

    [Title("Goods Tooltip")]
    [SerializeField, ReadOnly]
    private GoodsTooltip goodsTooltip;

    [Line]
    [SerializeField, Required]
    private Timer producedTimer;

    [Line]
    [SerializeField]
    private bool hideGoodsTooltipOnDrag;

    [SerializeField, ReadOnly]
    private bool isProducing;

    public override void Initialize(BuildingSystem buildingSystem, GridLayout gridLayout,
        TimerTooltip tooltip)
    {
        base.Initialize(buildingSystem, gridLayout, tooltip);
        InitializeProducers();
        this.producedTimer.Initialize(false);
        this.producedTimer.OnFinished += OnProducedFinishedHandler;
        this.producedTimer.OnHeartbeat += OnHeartbeatHandler;
    }


    private void OnProducedFinishedHandler()
    {
        this.timerTooltip.Hide();
        this.graphic.sprite = this.currentRecipe.growingGraphics.Last();
    }

    private void OnHeartbeatHandler(int time)
    {
        this.graphic.sprite = this.currentRecipe.growingGraphics[time - 1];
    }

    private void InitializeProducers()
    {
        foreach (ProducedSlot producer in this.slots)
        {
            producer.OnAddRecipe += OnAddRecipeHandler;
        }
    }

    private void OnAddRecipeHandler(GoodsRecipe recipe)
    {
        if (this.hideGoodsTooltipOnDrag)
        {
            this.goodsTooltip.HideContent();
        }

        this.productionQueue.Enqueue(recipe);
        if (this.isProducing) return;
        this.isProducing = true;
        Produce();
    }

    private async void Produce()
    {
        while (this.productionQueue.Count > 0)
        {
            this.currentRecipe = this.productionQueue.Dequeue();
            await ProduceCurrentRecipe();
        }

        this.isProducing = false;
    }

    private async UniTask ProduceCurrentRecipe()
    {
        this.graphic.sprite = this.currentRecipe.growingGraphics[0];
        TimeSpan producedTime = new TimeSpan(
            this.currentRecipe.days,
            this.currentRecipe.hours,
            this.currentRecipe.minutes,
            this.currentRecipe.seconds);
        int heartbeat = this.currentRecipe.growingGraphics.Count - 1;
        this.producedTimer.StartTimer(string.Empty, producedTime, heartbeat);
        await UniTask.Delay(producedTime);
    }


    public void SetTooltip(GoodsTooltip tooltip)
    {
        this.goodsTooltip = tooltip;
    }

    protected override void ShowTooltips()
    {
        base.ShowTooltips();
        ShowGoodsTooltip();
        ShowProductionTimer();
    }

    private void ShowGoodsTooltip()
    {
        if (this.isBuilding) return;
        if (this.isProducing) return;
        this.goodsTooltip.Show(this.recipes, transform);
    }

    private void ShowProductionTimer()
    {
        if (this.isProducing)
        {
            this.timerTooltip.Show(this.producedTimer);
        }
    }
}