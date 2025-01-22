using Dt.Attribute;
using Lean.Touch;
using UnityEngine;

public class OpeningViewFacility : Facility
{
    [SerializeField]
    private OpenableView view;

    [SerializeField, ReadOnly]
    private bool isOverUI;

    public override void Initialize(BuildingSystem buildingSystem, GridLayout gridLayout,
        TimerTooltip tooltip, ItemInfo info, FacilityData data = null)
    {
        base.Initialize(buildingSystem, gridLayout, tooltip, info, data);
        LeanTouch.OnFingerDown += OnFingerDownHandler;
    }

    private void OnFingerDownHandler(LeanFinger finger)
    {
        this.isOverUI = finger.IsOverGui;
    }

    protected override void OnFingerTapHandler()
    {
        base.OnFingerTapHandler();
        if (this.isOverUI) return;
        Messenger.Broadcast(Message.OpenView, this.view);
    }
}