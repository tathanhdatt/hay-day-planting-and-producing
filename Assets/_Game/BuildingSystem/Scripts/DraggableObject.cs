using System;
using Dt.Attribute;
using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private Camera cam;

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
        this.cam = Camera.main;
        this.gridLayout = gridLayout;
        this.buildingSystem = buildingSystem;
        this.bounds = bounds;
        this.isPlaced = false;
        enabled = true;
    }

    private void OnEnable()
    {
        GameState.isEditing = true;
    }

    private void OnDisable()
    {
        GameState.isEditing = false;
    }

    public void ClearAndSaveOldPosition()
    {
        UpdateBoundsPosition();
        this.oldPosition = transform.position;
        this.buildingSystem.ClearTilesInArea(this.bounds);
    }

    private void Update()
    {
        MoveObjectByMouse();
    }

    private void MoveObjectByMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = this.cam.ScreenToWorldPoint(mousePos);
        worldPos.z = 0;
        Vector3Int gridPos = this.gridLayout.WorldToCell(worldPos);
        transform.position = this.gridLayout.CellToLocalInterpolated(gridPos);
    }

    private void LateUpdate()
    {
        if (!Input.GetMouseButtonUp(0)) return;
        UpdateBoundsPosition();
        TryPlace();
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
}