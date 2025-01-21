using System;
using Dt.Attribute;
using Lean.Touch;
using UnityEngine;

public class InteractionDetector : MonoBehaviour
{
    [SerializeField, Required]
    private Collider2D holdingCollider;

    [SerializeField]
    private float holdingDuration;

    [SerializeField, ReadOnly]
    private float holdingTimeSpan;

    [SerializeField, ReadOnly]
    private bool isHolding;

    [SerializeField, ReadOnly]
    private bool isOverGui;

    public event Action OnAllowedEditing;
    public event Action OnFingerTap;

    public void Initialize()
    {
        LeanTouch.OnFingerDown += OnFingerDownHandler;
        LeanTouch.OnFingerUpdate += OnFingerUpdateHandler;
        LeanTouch.OnFingerUp += OnFingerUpHandler;
        LeanTouch.OnFingerTap += OnFingerTapHandler;
    }


    private void OnFingerDownHandler(LeanFinger finger)
    {
        this.isOverGui = finger.IsOverGui;
        if (this.isOverGui) return;
        if (!IsCurrentFingerInBounds(finger)) return;
        this.holdingTimeSpan = 0;
        this.isHolding = true;
    }

    private void OnFingerUpdateHandler(LeanFinger finger)
    {
        bool isFingerMoved = finger.ScreenDelta != Vector2.zero;
        if (isFingerMoved)
        {
            OnFingerMoved();
        }
        else
        {
            CooldownHolding();
        }
    }

    private void OnFingerUpHandler(LeanFinger finger)
    {
        this.holdingTimeSpan = 0;
        this.isHolding = false;
    }

    private void OnFingerMoved()
    {
        this.isHolding = false;
    }

    private void OnFingerTapHandler(LeanFinger finger)
    {
        if (this.isOverGui) return;
        if (IsCurrentFingerInBounds(finger))
        {
            OnFingerTap?.Invoke();
        }
    }

    private bool IsCurrentFingerInBounds(LeanFinger finger)
    {
        Vector3 worldPos = finger.GetWorldPosition(CameraConstant.ZPosition);
        worldPos.z = 0;
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector3.forward, 1);
        return hit.collider == this.holdingCollider;
    }

    private void CooldownHolding()
    {
        if (!this.isHolding) return;
        IncreaseTimeSpan();
        if (IsEnoughHoldingTime())
        {
            AllowEditing();
        }
    }

    private void IncreaseTimeSpan()
    {
        this.holdingTimeSpan += Time.deltaTime;
    }

    private bool IsEnoughHoldingTime()
    {
        return this.holdingTimeSpan >= this.holdingDuration;
    }

    private void AllowEditing()
    {
        this.isHolding = false;
        OnAllowedEditing?.Invoke();
    }

    private void OnDestroy()
    {
        LeanTouch.OnFingerDown -= OnFingerDownHandler;
        LeanTouch.OnFingerUpdate -= OnFingerUpdateHandler;
        LeanTouch.OnFingerUp -= OnFingerUpHandler;
        LeanTouch.OnFingerTap -= OnFingerTapHandler;
    }
}