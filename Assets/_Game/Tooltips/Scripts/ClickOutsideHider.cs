using System.Collections.Generic;
using Dt.Attribute;
using Lean.Touch;
using UnityEngine;

public class ClickOutsideHider : MonoBehaviour
{
    [SerializeField, Required]
    private List<RectTransform> contents;

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
        foreach (RectTransform content in this.contents)
        {
            Vector3 localPos = content.InverseTransformPoint(fingerPos);
            localPos.z = 0;
            if (content.rect.Contains(localPos)) return;
        }

        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}