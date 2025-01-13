using System;
using Dt.Attribute;
using Lean.Touch;
using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    [SerializeField, Required]
    private SpriteRenderer graphic;

    [SerializeField, ReadOnly]
    private GridLayout gridLayout;

    [SerializeField, ReadOnly]
    private BuildingSystem buildingSystem;

    [SerializeField, ReadOnly]
    private bool isPlaced;

    [SerializeField, ReadOnly]
    private Vector3 oldPosition;

    private BoundsInt bounds;
    public event Action OnFirstTimePlaced;

    public void Initialize(BuildingSystem buildingSystem, GridLayout gridLayout, BoundsInt bounds)
    {
        this.gridLayout = gridLayout;
        this.buildingSystem = buildingSystem;
        this.bounds = bounds;
        this.isPlaced = false;
        enabled = true;
    }


    private void OnEnable()
    {
        GameState.isEditing = true;
        SetInteractingLayer();
        LeanTouch.OnFingerUpdate += OnFingerUpdateHandler;
        LeanTouch.OnFingerUp += OnFingerUpHandler;
    }

    private void OnDisable()
    {
        GameState.isEditing = false;
        ResetSortingLayer();
        LeanTouch.OnFingerUpdate -= OnFingerUpdateHandler;
        LeanTouch.OnFingerUp -= OnFingerUpHandler;
    }

    private void SetInteractingLayer()
    {
        this.graphic.sortingLayerName = SortingLayerName.Interactable;
    }

    private void ResetSortingLayer()
    {
        this.graphic.sortingLayerName = SortingLayerName.Facility;
    }

    private void OnFingerUpdateHandler(LeanFinger finger)
    {
        Vector3 worldPos = finger.GetWorldPosition(CameraConstant.ZPosition);
        worldPos.z = 0;
        Vector3Int gridPos = this.gridLayout.WorldToCell(worldPos);
        transform.position = this.gridLayout.CellToLocalInterpolated(gridPos);
    }

    private void OnFingerUpHandler(LeanFinger finger)
    {
        UpdateBoundsPosition();
        TryPlace();
    }

    public void ClearAndSaveOldPosition()
    {
        UpdateBoundsPosition();
        this.oldPosition = transform.position;
        this.buildingSystem.ClearTilesInArea(this.bounds);
    }

    private void TryPlace()
    {
        if (this.isPlaced)
        {
            TryReplace();
        }
        else
        {
            PlaceFirstTime();
        }
    }

    private void TryReplace()
    {
        if (this.buildingSystem.CanPlace(this.bounds))
        {
            Place();
        }
        else
        {
            transform.position = this.oldPosition;
            UpdateBoundsPosition();
            Place();
        }
    }

    private void PlaceFirstTime()
    {
        if (this.buildingSystem.CanPlace(this.bounds))
        {
            OnFirstTimePlaced?.Invoke();
            this.isPlaced = true;
            Place();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Place()
    {
        this.buildingSystem.PlaceTilesInArea(this.bounds);
        enabled = false;
    }

    private void UpdateBoundsPosition()
    {
        Vector3Int pos = this.gridLayout.LocalToCell(transform.position);
        this.bounds.position = pos;
    }

    private void OnDestroy()
    {
    }
}