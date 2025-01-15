using Dt.Attribute;
using Lean.Touch;
using UnityEngine;

public class OpeningViewFacility : Facility
{
    [SerializeField]
    private OpenableView view;

    [SerializeField, ReadOnly]
    private bool canOpen;

    protected override void OnFingerUpHandler(LeanFinger finger)
    {
        if (!this.canOpen) return;
        base.OnFingerUpHandler(finger);
        Messenger.Broadcast(Message.OpenView, this.view);
    }

    protected override void OnFingerMoveHandler(LeanFinger finger)
    {
        base.OnFingerMoveHandler(finger);
        this.canOpen = false;
    }


    protected override void OnFingerDownHandler(LeanFinger finger)
    {
        base.OnFingerDownHandler(finger);
        this.canOpen = true;
    }
}