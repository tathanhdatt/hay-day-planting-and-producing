using Cysharp.Threading.Tasks;
using Dt.Attribute;
using UnityEngine;

public class CropFacility : GoodsFacility
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

    public override void Initialize(BuildingSystem buildingSystem,
        GridLayout gridLayout, TimerTooltip tooltip)
    {
        base.Initialize(buildingSystem, gridLayout, tooltip);
        RegisterFreeSlotEvents();
        this.producedTimer.OnHeartbeat += OnHeartbeatHandler;
    }

    private void RegisterFreeSlotEvents()
    {
        foreach (ProducedSlot slot in this.slots)
        {
            slot.OnFreeSlot += OnFreeSlotHandler;
        }
    }

    private async void OnFreeSlotHandler()
    {
        this.graphic.sprite = this.currentRecipe.finishedGraphic;
        await UniTask.WaitForSeconds(0.6f);
        this.graphic.sprite = this.defaultGraphic;
        this.canHarvest = false;
    }

    private void OnHeartbeatHandler(int time)
    {
        this.graphic.sprite = this.currentRecipe.growingGraphics[time - 1];
    }

    protected override void OnProducedFinishedHandler()
    {
        base.OnProducedFinishedHandler();
        this.canHarvest = true;
        this.graphic.sprite = this.currentRecipe.growingGraphics.Last();
    }

    protected override async UniTask ProduceCurrentRecipe()
    {
        this.graphic.sprite = this.currentRecipe.growingGraphics[0];
        await base.ProduceCurrentRecipe();
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