﻿using System;
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

    public event Action OnAllowedEditing;
    public event Action OnTouchedOut;
    public event Action OnTouchedMoved;


    public void Initialize()
    {
        LeanTouch.OnFingerDown += OnFingerDownHandler;
        LeanTouch.OnFingerUpdate += OnFingerUpdateHandler;
        LeanTouch.OnFingerUp += OnFingerUpHandler;
        LeanTouch.OnFingerTap += OnFingerTapHandler;
    }


    private void OnFingerDownHandler(LeanFinger finger)
    {
        if (finger.IsOverGui) return;
        if (IsCurrentFingerInBounds(finger))
        {
            this.holdingTimeSpan = 0;
            this.isHolding = true;
        }
        else
        {
            OnTouchedOut?.Invoke();
        }
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

    private void OnFingerTapHandler(LeanFinger finger)
    {
        if (IsCurrentFingerInBounds(finger))
        {
            Messenger.Broadcast(Message.MoveCameraTo, transform.position);
        }
    }

    private void OnFingerMoved()
    {
        OnTouchedMoved?.Invoke();
        this.isHolding = false;
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