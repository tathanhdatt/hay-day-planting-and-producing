﻿using System;
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
    public event Action OnPlaced;

    public void Initialize(BuildingSystem buildingSystem, GridLayout gridLayout, BoundsInt bounds)
    {
        this.gridLayout = gridLayout;
        this.buildingSystem = buildingSystem;
        this.bounds = bounds;
        this.isPlaced = false;
    }

    private void OnEnable()
    {
        GameState.isEditing = true;
        LeanTouch.OnFingerUpdate += OnFingerUpdateHandler;
        LeanTouch.OnFingerUp += OnFingerUpHandler;
        SetInteractingLayer();
    }

    private void OnDisable()
    {
        GameState.isEditing = false;
        LeanTouch.OnFingerUpdate -= OnFingerUpdateHandler;
        LeanTouch.OnFingerUp -= OnFingerUpHandler;
        ResetSortingLayer();
    }

    private void SetInteractingLayer()
    {
        this.graphic.sortingLayerName = SortingLayerName.Interactable;
    }

    private void ResetSortingLayer()
    {
        this.graphic.sortingLayerName = SortingLayerName.Facility;
    }

    public void OnFingerUpdateHandler(LeanFinger finger)
    {
        if (enabled)
        {
            UpdatePosition(finger);
        }
    }

    private void UpdatePosition(LeanFinger finger)
    {
        Vector3 worldPos = finger.GetWorldPosition(CameraConstant.ZPosition);
        worldPos.z = 0;
        SetPosition(worldPos);
    }

    public void SetPosition(Vector3 position)
    {
        Vector3Int gridPos = this.gridLayout.WorldToCell(position);
        transform.position = this.gridLayout.CellToLocalInterpolated(gridPos);
    }

    public void OnFingerUpHandler(LeanFinger finger)
    {
        if (enabled)
        {
            TryPlace();
        }
    }

    public void ClearAndSaveOldPosition()
    {
        UpdateBoundsPosition();
        this.oldPosition = transform.position;
        this.buildingSystem.ClearTilesInArea(this.bounds);
    }

    public void TryPlace()
    {
        UpdateBoundsPosition();
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

    public void PlaceFirstTime()
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
        OnPlaced?.Invoke();
    }

    private void UpdateBoundsPosition()
    {
        Vector3Int pos = this.gridLayout.LocalToCell(transform.position);
        this.bounds.position = pos;
    }

    public void SetPlaced(bool placed)
    {
        this.isPlaced = placed;
    }
}