using System;
using Dt.Attribute;
using Lean.Touch;
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

    [SerializeField, ReadOnly]
    protected bool isBuilding;

    [SerializeField, ReadOnly]
    private TimerTooltip timerTooltip;

    public event Action OnFirstTimePlaced;

    public void Initialize(BuildingSystem buildingSystem, GridLayout gridLayout,
        TimerTooltip tooltip)
    {
        this.timerTooltip = tooltip;
        this.isBuilding = false;
        InitializeBuildingTimer();
        InitializeInteractionDetector();
        InitializeDraggableObject(buildingSystem, gridLayout);
    }

    private void InitializeBuildingTimer()
    {
        if (this.buildingTimer == null) return;
        this.buildingTimer.Initialize();
        this.buildingTimer.OnFinished += OnFinishedHandler;
    }

    private void OnFinishedHandler()
    {
        this.buildingTimer.OnFinished -= OnFinishedHandler;
        this.isBuilding = false;
    }

    private void InitializeInteractionDetector()
    {
        this.interactionDetector.Initialize();
        this.interactionDetector.OnAllowedEditing += OnAllowedEditingHandler;
        this.interactionDetector.OnFingerDownOut += OnFingerDownOutHandler;
        this.interactionDetector.OnFingerMove += OnFingerMoveHandler;
        this.interactionDetector.OnFingerDown += OnFingerDownHandler;
        this.interactionDetector.OnFingerUp += OnFingerUpHandler;
        this.interactionDetector.OnFingerUpdate += OnFingerUpdateHandler;
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
        this.isBuilding = true;
        this.buildingTimer.StartTimer("Building", buildingTimeSpan);
    }

    private void OnFirstTimePlacedHandler()
    {
        this.draggableObject.OnFirstTimePlaced -= OnFirstTimePlacedHandler;
        OnFirstTimePlaced?.Invoke();
    }

    protected virtual void OnFingerDownOutHandler()
    {
        if (this.timerTooltip.CurrentTimer == this.buildingTimer)
        {
            this.timerTooltip.Hide();
        }
    }

    protected virtual void OnFingerDownHandler(LeanFinger finger)
    {
    }

    protected virtual void OnFingerMoveHandler(LeanFinger finger)
    {
        this.timerTooltip.Hide();
    }


    protected virtual void OnFingerUpHandler(LeanFinger finger)
    {
        this.draggableObject?.OnFingerUpHandler(finger);
        if (this.isBuilding)
        {
            ShowBuildingTimer();
        }
    }

    protected virtual void OnFingerUpdateHandler(LeanFinger finger)
    {
        this.draggableObject?.OnFingerUpdateHandler(finger);
    }

    private void ShowBuildingTimer()
    {
        this.timerTooltip.Show(this.buildingTimer);
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
        this.interactionDetector.OnFingerUpdate -= OnFingerUpdateHandler;
    }
}