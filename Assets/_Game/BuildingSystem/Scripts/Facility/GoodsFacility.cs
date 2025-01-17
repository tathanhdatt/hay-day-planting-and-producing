﻿using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Dt.Attribute;
using UnityEngine;

public abstract class GoodsFacility : Facility
{
    private readonly Queue<GoodsRecipe> productionQueue = new Queue<GoodsRecipe>(10);

    [Title("Goods facility")]
    [SerializeField, Required]
    protected SpriteRenderer graphic;

    [SerializeField]
    private List<GoodsRecipe> recipes;

    [SerializeField, ReadOnly]
    protected GoodsRecipe currentRecipe;

    [SerializeField]
    protected List<ProducedSlot> slots;

    [SerializeField, Required]
    protected Timer producedTimer;

    [Title("Goods Tooltip")]
    [SerializeField, ReadOnly]
    private GoodsTooltip goodsTooltip;

    [SerializeField]
    private bool hideGoodsTooltipOnDrag;

    [SerializeField, ReadOnly]
    protected bool isProducing;

    public override void Initialize(BuildingSystem buildingSystem, GridLayout gridLayout,
        TimerTooltip tooltip)
    {
        base.Initialize(buildingSystem, gridLayout, tooltip);
        InitializeSlots();
        this.producedTimer.Initialize(false);
        this.producedTimer.OnFinished += OnProducedFinishedHandler;
    }


    protected virtual void OnProducedFinishedHandler()
    {
        this.isProducing = false;
        this.slots.First().SetIsProducing(false);
        this.timerTooltip.Hide();
        this.graphic.sprite = this.currentRecipe.growingGraphics.Last();
    }

    private void InitializeSlots()
    {
        foreach (ProducedSlot slot in this.slots)
        {
            slot.enabled = false;
            slot.OnAddRecipe += OnAddRecipeHandler;
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
        Produce();
    }

    protected override void OnFinishedBuildHandler()
    {
        base.OnFinishedBuildHandler();
        EnableProducedSlots();
    }

    private void EnableProducedSlots()
    {
        foreach (ProducedSlot slot in this.slots)
        {
            slot.enabled = true;
        }
    }

    private async void Produce()
    {
        while (this.productionQueue.Count > 0)
        {
            this.currentRecipe = this.productionQueue.Dequeue();
            if (IsEnoughRecipes())
            {
                await ProduceCurrentRecipe();
            }
            else
            {
                // TODO: Notify not enough recipes
            }
        }

        this.isProducing = false;
    }

    private bool IsEnoughRecipes()
    {
        foreach (GoodsRequirement requirement in this.currentRecipe.recipes)
        {
            if (requirement.requiredQuantity > requirement.goods.quantity)
            {
                return false;
            }
        }

        return true;
    }

    private async UniTask ProduceCurrentRecipe()
    {
        this.slots.First().SetIsProducing(true);
        this.graphic.sprite = this.currentRecipe.growingGraphics[0];
        TimeSpan producedTime = new TimeSpan(
            this.currentRecipe.days,
            this.currentRecipe.hours,
            this.currentRecipe.minutes,
            this.currentRecipe.seconds);
        int heartbeat = this.currentRecipe.growingGraphics.Count - 1;
        this.isProducing = true;
        this.producedTimer.StartTimer(string.Empty, producedTime, heartbeat);
        await UniTask.WaitUntil(() => !this.isProducing);
    }

    public void SetGoodsTooltip(GoodsTooltip tooltip)
    {
        this.goodsTooltip = tooltip;
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
        this.goodsTooltip.Show(this.recipes, transform);
    }
}