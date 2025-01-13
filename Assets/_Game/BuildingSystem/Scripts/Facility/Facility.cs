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


    [SerializeField]
    private BoundsInt bounds;

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
        this.buildingTimer.Initialize();
    }

    private void InitializeInteractionDetector()
    {
        this.interactionDetector.Initialize();
        this.interactionDetector.OnAllowedEditing += OnAllowedEditingHandler;
        this.interactionDetector.OnFingerDownOut += OnFingerDownOutHandler;
        this.interactionDetector.OnFingerMove += OnFingerMoveHandler;
        this.interactionDetector.OnFingerDown += OnFingerDownHandler;
    }

    private void InitializeDraggableObject(BuildingSystem buildingSystem, GridLayout gridLayout)
    {
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

    private void OnFingerMoveHandler()
    {
        this.tooltip.Hide();
    }

    protected abstract void OnFingerDownHandler();

    private void OnDestroy()
    {
        this.draggableObject.OnFirstTimePlaced -= OnFirstTimePlacedHandler;
        this.interactionDetector.OnAllowedEditing -= OnAllowedEditingHandler;
        this.interactionDetector.OnFingerDownOut -= OnFingerDownOutHandler;
        this.interactionDetector.OnFingerMove -= OnFingerMoveHandler;
        this.interactionDetector.OnFingerDown -= OnFingerDownHandler;
    }
}