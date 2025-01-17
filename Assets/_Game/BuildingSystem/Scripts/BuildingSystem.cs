using System;
using System.Collections.Generic;
using Dt.Attribute;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingSystem : MonoBehaviour
{
    [SerializeField, Required]
    private GridLayout gridLayout;

    [SerializeField, Required]
    private Tilemap mainTilemap;

    [SerializeField, Required]
    private TileBase occupiedTile;
    
    [Title("Tooltip")]
    [SerializeField, Required]
    private TimerTooltip timerTooltip;
    
    [SerializeField, Required]
    private GoodsTooltip goodsTooltip;
    
    [SerializeField, Required]
    private HarvestTooltip harvestTooltip;

    [Line]
    [SerializeField]
    private List<Facility> availableFacilities;

    private Facility facility;

    private ICurrency currency;
    private ShopItemInfo currentItemInfo;

    public void Initialize(ICurrency currency)
    {
        Messenger.AddListener<ShopItemInfo>(Message.SpawnItem, SpawnItemHandler);
        this.currency = currency;
        InitializeAvailableFacilities();
    }

    private void InitializeAvailableFacilities()
    {
        foreach (Facility availableFacility in this.availableFacilities)
        {
            availableFacility.Initialize(this, this.gridLayout, this.timerTooltip);
            availableFacility.SetDraggable(false);
            availableFacility.SetPlaced(true);
        }
    }

    private void SpawnItemHandler(ShopItemInfo info)
    {
        this.currentItemInfo = info;
        InitializeFacility();
    }

    private void InitializeFacility()
    {
        this.facility = Instantiate(
            this.currentItemInfo.prefab, this.mainTilemap.transform);
        this.facility.Initialize(this, this.gridLayout, this.timerTooltip);
        this.facility.SetDraggable(true);
        if (this.facility is GoodsFacility goodsFacility)
        {
            goodsFacility.SetGoodsTooltip(this.goodsTooltip);
        }

        if (this.facility is CropFacility cropFacility)
        {
            cropFacility.SetHarvestTooltip(this.harvestTooltip);
        }
        this.facility.OnFirstTimePlaced += OnFirstTimePlacedHandler;
    }

    private void Build()
    {
        TimeSpan buildingTimeSpan = new TimeSpan(
            this.currentItemInfo.days,
            this.currentItemInfo.hours,
            this.currentItemInfo.minutes,
            this.currentItemInfo.seconds);
        this.facility.StartBuilding(buildingTimeSpan);
    }

    private void OnFirstTimePlacedHandler()
    {
        this.facility.OnFirstTimePlaced -= OnFirstTimePlacedHandler;
        this.currentItemInfo.IncreaseQuantity();
        SubtractAmount();
        Build();
    }

    private void SubtractAmount()
    {
        CurrencyType type = this.currentItemInfo.currencyType;
        int price = this.currentItemInfo.price;
        this.currency.SubtractAmount(type, price);
    }

    private TileBase[] GetTilesInArea(BoundsInt area)
    {
        TileBase[] tiles = new TileBase[area.size.x * area.size.y];
        int count = 0;
        foreach (Vector3Int pos in area.allPositionsWithin)
        {
            tiles[count++] = this.mainTilemap.GetTile(pos);
        }

        return tiles;
    }

    private void SetTilesInArea(BoundsInt area, TileBase tileBase)
    {
        foreach (Vector3Int pos in area.allPositionsWithin)
        {
            this.mainTilemap.SetTile(pos, tileBase);
        }
    }

    public void ClearTilesInArea(BoundsInt area)
    {
        SetTilesInArea(area, null);
    }

    public void PlaceTilesInArea(BoundsInt area)
    {
        SetTilesInArea(area, this.occupiedTile);
    }

    public bool CanPlace(BoundsInt area)
    {
        TileBase[] tileBases = GetTilesInArea(area);
        foreach (TileBase tileBase in tileBases)
        {
            if (tileBase == this.occupiedTile)
            {
                return false;
            }
        }

        return true;
    }
}