using System;
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
    
    [Line]
    [SerializeField, Required]
    private TimerTooltip tooltip;

    private Facility facility;

    private ICurrency currency;
    private ShopItemInfo currentItemInfo;

    public void Initialize(ICurrency currency)
    {
        Messenger.AddListener<ShopItemInfo>(Message.SpawnItem, SpawnItemHandler);
        this.currency = currency;
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
        this.facility.Initialize(this, this.gridLayout, this.tooltip);
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