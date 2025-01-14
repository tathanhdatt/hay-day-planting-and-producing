using System;
using Dt.Attribute;
using UnityEngine;

public abstract class Facility : MonoBehaviour
{
    [SerializeField, Required]
    private InteractionDetector interactionDetector;

    [SerializeField]
    private DraggableObject draggableObject;

    [SerializeField]
    private Timer buildingTimer;

    [SerializeField, Required]
    private SpriteRenderer graphic;

    [SerializeField]
    private BoundsInt bounds;

    [SerializeField, ReadOnly]
    private bool canFocus;

    private TimerTooltip tooltip;

    public event Action OnFirstTimePlaced;

    public void Initialize(BuildingSystem buildingSystem, GridLayout gridLayout,
        TimerTooltip tooltip)
    {
        this.tooltip = tooltip;
        InitializeBuildingTimer();
        InitializeInteractionDetector();
        InitializeDraggableObject(buildingSystem, gridLayout);
    }

    private void InitializeBuildingTimer()
    {
        this.buildingTimer?.Initialize();
    }

    private void InitializeInteractionDetector()
    {
        this.interactionDetector.Initialize();
        this.interactionDetector.OnAllowedEditing += OnAllowedEditingHandler;
        this.interactionDetector.OnFingerDownOut += OnFingerDownOutHandler;
        this.interactionDetector.OnFingerMove += OnFingerMoveHandler;
        this.interactionDetector.OnFingerDown += OnFingerDownHandler;
        this.interactionDetector.OnFingerUp += OnFingerUpHandler;
    }

    private void InitializeDraggableObject(BuildingSystem buildingSystem, GridLayout gridLayout)
    {
        if (this.draggableObject == null) return;
        this.draggableObject.Initialize(buildingSystem, gridLayout, this.bounds);
        this.draggableObject.OnFirstTimePlaced += OnFirstTimePlacedHandler;
    }

    private void OnAllowedEditingHandler()
    {
        if (this.draggableObject == null) return;
        this.draggableObject.ClearAndSaveOldPosition();
        this.draggableObject.enabled = true;
    }

    public void StartBuilding(TimeSpan buildingTimeSpan)
    {
        this.buildingTimer.StartTimer("Building", buildingTimeSpan);
    }

    private void OnFirstTimePlacedHandler()
    {
        this.draggableObject.OnFirstTimePlaced -= OnFirstTimePlacedHandler;
        OnFirstTimePlaced?.Invoke();
    }

    private void OnMouseUpAsButton()
    {
        this.tooltip.Show(this.buildingTimer);
    }

    private void OnFingerDownOutHandler()
    {
        if (this.tooltip.CurrentTimer == this.buildingTimer)
        {
            this.tooltip.Hide();
        }
    }

    protected virtual void OnFingerDownHandler()
    {
        this.canFocus = true;
    }

    protected virtual void OnFingerMoveHandler()
    {
        this.canFocus = false;
        this.tooltip.Hide();
    }


    protected virtual void OnFingerUpHandler()
    {
        if (this.canFocus)
        {
            Focus();
        }
    }

    private void Focus()
    {
        Vector3 center = transform.position;
        center.y += this.graphic.bounds.size.y / 2;
        Messenger.Broadcast(Message.MoveCameraTo, center);
    }

    public void SetDraggable(bool draggable)
    {
        if (this.draggableObject == null) return;
        this.draggableObject.enabled = draggable;
    }

    public void SetPlaced(bool placed)
    {
        this.draggableObject?.SetPlaced(placed);
    }

    private void OnDestroy()
    {
        if (this.draggableObject != null)
        {
            this.draggableObject.OnFirstTimePlaced -= OnFirstTimePlacedHandler;
        }

        this.interactionDetector.OnAllowedEditing -= OnAllowedEditingHandler;
        this.interactionDetector.OnFingerDownOut -= OnFingerDownOutHandler;
        this.interactionDetector.OnFingerMove -= OnFingerMoveHandler;
        this.interactionDetector.OnFingerDown -= OnFingerDownHandler;
        this.interactionDetector.OnFingerUp -= OnFingerUpHandler;
    }
}