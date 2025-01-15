using System.Collections.Generic;
using Dt.Attribute;
using Lean.Touch;
using UnityEngine;

public class ProductionFacility : Facility
{
    [Title("Production facility")]
    [SerializeField]
    private List<GoodsRecipe> recipes;

    [SerializeField, ReadOnly]
    private GoodsTooltip goodsTooltip;

    [SerializeField, ReadOnly]
    private bool canShowGoodsTooltip;

    public void SetTooltip(GoodsTooltip tooltip)
    {
        this.goodsTooltip = tooltip;
    }

    protected override void OnFingerUpHandler(LeanFinger finger)
    {
        base.OnFingerUpHandler(finger);
        if (this.isBuilding) return;
        ShowGoodsTooltip();
    }

    private void ShowGoodsTooltip()
    {
        if (this.canShowGoodsTooltip)
        {
            this.goodsTooltip.Show(this.recipes, transform);
        }
    }

    protected override void OnFingerUpdateHandler(LeanFinger finger)
    {
        base.OnFingerUpdateHandler(finger);
        if (GameState.isEditing)
        {
            this.canShowGoodsTooltip = false;
        }
    }

    protected override void OnFingerMoveHandler(LeanFinger finger)
    {
        base.OnFingerMoveHandler(finger);
        this.goodsTooltip.Hide();
    }

    protected override void OnFingerDownHandler(LeanFinger finger)
    {
        base.OnFingerDownHandler(finger);
        this.canShowGoodsTooltip = true;
    }

    protected override void OnFingerDownOutHandler()
    {
        base.OnFingerDownOutHandler();
        this.goodsTooltip.Hide();
    }
}