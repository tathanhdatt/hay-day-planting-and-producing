using Dt.Attribute;
using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    [SerializeField, Required]
    private SpriteRenderer graphic;

    [SerializeField]
    private BoundsInt bounds;

    [SerializeField]
    private BoxCollider2D holdingCollider;

    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private float holdingDuration;

    [SerializeField, ReadOnly]
    private DraggableObject draggableObject;

    [SerializeField, ReadOnly]
    private float holdingTimeSpan;

    [SerializeField, ReadOnly]
    private bool isHolding;

    [SerializeField, ReadOnly]
    private Camera cam;

    public BoundsInt Bounds => this.bounds;

    public void Initialize(DraggableObject draggableObject)
    {
        this.draggableObject = draggableObject;
        this.cam = Camera.main;
    }

    private void Update()
    {
        DetectHolding();
        CooldownHolding();
    }

    private void DetectHolding()
    {
        bool hasTouch = Input.touchCount > 0;
        if (!hasTouch) return;
        Touch touch = Input.GetTouch(0);
        switch (touch.phase)
        {
            case TouchPhase.Began:
                OnTouchBegan(touch);
                break;
            case TouchPhase.Ended:
                OnTouchEnded(touch);
                break;
            case TouchPhase.Moved:
                OnTouchMoved(touch);
                break;
        }
    }

    private void OnTouchBegan(Touch touch)
    {
        if (!IsInBounds(touch)) return;
        this.holdingTimeSpan = 0;
        this.isHolding = true;
    }

    private void OnTouchMoved(Touch touch)
    {
        this.isHolding = false;
    }

    private void OnTouchEnded(Touch touch)
    {
        this.isHolding = false;
    }

    private bool IsInBounds(Touch touch)
    {
        Vector3 worldPos = this.cam.ScreenToWorldPoint(touch.position);
        worldPos.z = 0;
        return this.holdingCollider.bounds.Contains(worldPos);
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
        this.draggableObject.enabled = true;
        this.draggableObject.ClearAndSaveOldPosition();
        this.isHolding = false;
    }
}