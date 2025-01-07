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
        if (!Input.GetMouseButtonDown(0)) return;
        if (!IsInBounds()) return;
        this.holdingTimeSpan = 0;
        this.isHolding = true;
    }

    private bool IsInBounds()
    {
        Vector3 worldPos = this.cam.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0;
        return this.holdingCollider.bounds.Contains(worldPos);
    }

    private void CooldownHolding()
    {
        if (!this.isHolding) return;
        this.holdingTimeSpan += Time.deltaTime;
        bool isEnoughTime = this.holdingTimeSpan >= this.holdingDuration;
        if (!isEnoughTime) return;
        this.draggableObject.enabled = true;
        this.draggableObject.ClearAndSaveOldPosition();
        this.isHolding = false;
    }
}