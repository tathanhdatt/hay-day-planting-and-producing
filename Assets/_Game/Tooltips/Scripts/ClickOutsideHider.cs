using Dt.Attribute;
using Lean.Touch;
using UnityEngine;

public class ClickOutsideHider : MonoBehaviour
{
    [SerializeField, Required]
    private RectTransform content;

    public void Initialize()
    {
        LeanTouch.OnFingerDown += OnFingerDownHandler;
    }

    private void OnFingerDownHandler(LeanFinger finger)
    {
        HideIfClickOutside(finger);
    }

    private void HideIfClickOutside(LeanFinger finger)
    {
        if (!gameObject.activeSelf) return;
        Vector3 fingerPos = finger.GetWorldPosition(CameraConstant.ZPosition);
        fingerPos = this.content.InverseTransformPoint(fingerPos);
        if (this.content.rect.Contains(fingerPos)) return;
        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}