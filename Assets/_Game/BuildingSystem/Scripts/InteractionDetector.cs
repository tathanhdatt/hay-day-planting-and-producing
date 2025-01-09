using System;
using Dt.Attribute;
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
    private Camera cam;

    private Touch touch;

    public event Action OnAllowedEditing;
    public event Action OnTouchedOut;
    public event Action OnTouchedMoved;


    public void Initialize()
    {
        this.cam = Camera.main;
    }

    private void Update()
    {
        DetectTouch();
        CooldownHolding();
    }

    private void DetectTouch()
    {
        bool hasTouch = Input.touchCount > 0;
        if (!hasTouch) return;
        this.touch = Input.GetTouch(0);
        switch (this.touch.phase)
        {
            case TouchPhase.Began:
                OnTouchBegan();
                break;
            case TouchPhase.Ended:
                OnTouchEnded();
                break;
            case TouchPhase.Moved:
                OnTouchMoved();
                break;
        }
    }

    private void OnTouchBegan()
    {
        if (!IsCurrentTouchInBounds()) return;
        this.holdingTimeSpan = 0;
        this.isHolding = true;
    }

    private void OnTouchMoved()
    {
        OnTouchedMoved?.Invoke();
        this.isHolding = false;
    }

    private void OnTouchEnded()
    {
        if (!IsCurrentTouchInBounds())
        {
            OnTouchedOut?.Invoke();
        }

        this.isHolding = false;
    }

    private bool IsCurrentTouchInBounds()
    {
        Vector3 worldPos = this.cam.ScreenToWorldPoint(this.touch.position);
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
}