using System;
using Dt.Attribute;
using UnityEngine;

public abstract class Facility : MonoBehaviour
{
    [SerializeField, Required]
    protected InteractionDetector interactionDetector;

    [SerializeField]
    private DraggableObject draggableObject;

    [SerializeField]
    private Timer buildingTimer;

    [SerializeField]
    private BoundsInt bounds;

    [Line]
    [SerializeField, ReadOnly]
    protected bool isBuilding;

    [SerializeField, ReadOnly]
    protected TimerTooltip timerTooltip;

    [SerializeField, ReadOnly]
    protected ItemInfo facilityInfo;

    protected FacilityData data;
    public event Action OnFirstTimePlaced;

    public virtual void Initialize(BuildingSystem buildingSystem, GridLayout gridLayout,
        TimerTooltip tooltip, ItemInfo info, FacilityData data = null)
    {
        this.timerTooltip = tooltip;
        this.isBuilding = false;
        this.facilityInfo = info;
        this.data = data;
        InitializeBuildingTimer();
        InitializeInteractionDetector();
        InitializeDraggableObject(buildingSystem, gridLayout);
    }

    private void InitializeBuildingTimer()
    {
        if (this.buildingTimer == null) return;
        this.buildingTimer.Initialize();
        this.buildingTimer.OnFinished += OnFinishedBuildHandler;
    }

    protected virtual void OnFinishedBuildHandler()
    {
        this.buildingTimer.OnFinished -= OnFinishedBuildHandler;
        this.isBuilding = false;
        this.timerTooltip.Hide();
        this.data.isBuilding = false;
    }

    private void InitializeInteractionDetector()
    {
        this.interactionDetector.Initialize();
        this.interactionDetector.OnAllowedEditing += OnAllowedEditingHandler;
        this.interactionDetector.OnFingerTap += OnFingerTapHandler;
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
        this.data.isBuilding = true;
        this.data.finishedBuildingTime = this.buildingTimer.GetFinishTimeString();
    }

    public void ContinueBuilding(DateTime finishTime)
    {
        TimeSpan remainBuildingTime = finishTime - DateTime.Now;
        TimeSpan totalBuildTime = new TimeSpan(
            this.facilityInfo.days,
            this.facilityInfo.hours,
            this.facilityInfo.minutes,
            this.facilityInfo.seconds);
        StartBuilding(totalBuildTime);
        this.buildingTimer.Subtract(totalBuildTime - remainBuildingTime);
    }

    private void OnFirstTimePlacedHandler()
    {
        this.draggableObject.OnFirstTimePlaced -= OnFirstTimePlacedHandler;
        UpdateData();
        OnFirstTimePlaced?.Invoke();
    }

    protected virtual void UpdateData()
    {
        this.data ??= new FacilityData();
        this.data.id = $"{this.facilityInfo.type.ToString()}";
        this.data.type = this.facilityInfo.type;
        this.data.position = transform.position;
    }

    protected virtual void OnFingerTapHandler()
    {
        ShowTooltips();
    }

    protected virtual void ShowTooltips()
    {
        ShowBuildingTimerTooltip();
    }

    private void ShowBuildingTimerTooltip()
    {
        if (this.isBuilding)
        {
            this.timerTooltip.Show(this.buildingTimer);
        }
    }

    public void TryPlace()
    {
        this.draggableObject.TryPlace();
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
        this.interactionDetector.OnFingerTap -= OnFingerTapHandler;
    }

    public virtual FacilityData GetData()
    {
        return this.data;
    }
}