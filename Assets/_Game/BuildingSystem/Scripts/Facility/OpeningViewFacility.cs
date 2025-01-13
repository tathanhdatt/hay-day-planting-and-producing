using UnityEngine;

public class OpeningViewFacility : Facility
{
    [SerializeField]
    private OpenableView view;

    protected override void OnFingerDownHandler()
    {
        Messenger.Broadcast(Message.OpenView, this.view);
    }
}