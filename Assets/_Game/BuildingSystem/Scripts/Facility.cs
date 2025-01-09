using System;
using Dt.Attribute;
using UnityEngine;

public class Facility : MonoBehaviour
{
    [SerializeField, Required]
    private InteractionDetector interactionDetector;

    [SerializeField, Required]
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
        this.buildingTimer.Initialize();
        this.interactionDetector.Initialize();
        this.interactionDetector.OnAllowedEditing += OnAllowedEditingHandler;
        this.interactionDetector.OnTouchedOut += OnTouchedOutHandler;
        this.interactionDetector.OnTouchedMoved += OnTouchedMovedHandler;
        this.draggableObject.Initialize(buildingSystem, gridLayout, this.bounds);
        this.draggableObject.OnFirstTimePlaced += OnFirstTimePlacedHandler;
    }


    private void OnAllowedEditingHandler()
    {
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

    private void OnTouchedOutHandler()
    {
        if (this.tooltip.CurrentTimer == this.buildingTimer)
        {
            this.tooltip.Hide();
        }
    }

    private void OnTouchedMovedHandler()
    {
        this.tooltip.Hide();
    }

    private void OnDestroy()
    {
        this.draggableObject.OnFirstTimePlaced -= OnFirstTimePlacedHandler;
        this.interactionDetector.OnAllowedEditing -= OnAllowedEditingHandler;
        this.interactionDetector.OnTouchedOut -= OnTouchedOutHandler;
        this.interactionDetector.OnTouchedMoved -= OnTouchedMovedHandler;
    }
}