using Dt.Attribute;
using UnityEngine;

public class OpeningViewFacility : Facility
{
    [SerializeField]
    private OpenableView view;

    [SerializeField, ReadOnly]
    private bool canOpen;

    protected override void OnFingerUpHandler()
    {
        if (!this.canOpen) return;
        base.OnFingerUpHandler();
        Messenger.Broadcast(Message.OpenView, this.view);
    }

    protected override void OnFingerMoveHandler()
    {
        base.OnFingerMoveHandler();
        this.canOpen = false;
    }


    protected override void OnFingerDownHandler()
    {
        base.OnFingerDownHandler();
        this.canOpen = true;
    }
}