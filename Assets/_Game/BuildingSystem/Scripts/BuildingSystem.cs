using System;
using System.Collections.Generic;
using System.Text;
using Dt.Attribute;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Windows;

public class BuildingSystem : MonoBehaviour
{
    private readonly List<Facility> placedFacilities = new List<Facility>(10);

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
    private CropTooltip cropTooltip;

    [SerializeField, Required]
    private ProductionTooltip productionTooltip;

    [SerializeField, Required]
    private HarvestTooltip harvestTooltip;

    [Line]
    [SerializeField]
    private List<Facility> availableFacilities;

    private Facility facility;
    private ItemInfo[] itemsInfos;
    private ICurrency currency;
    private ItemInfo currentItemInfo;
    private GoodsDatabase barnDatabase;

    public void Initialize(ICurrency currency, GoodsDatabase barnDatabase, string data)
    {
        Messenger.AddListener<ItemInfo>(Message.SpawnItem, SpawnItemHandler);
        this.currency = currency;
        this.barnDatabase = barnDatabase;
        LoadData(data);
        InitializeAvailableFacilities();
    }

    private void LoadData(string json)
    {
        if (string.IsNullOrEmpty(json)) return;
        BuildingSystemData data = JsonUtility.FromJson<BuildingSystemData>(json);
        this.itemsInfos = Resources.LoadAll<ItemInfo>("ShopItems");
        foreach (FacilityData fac in data.facilities)
        {
            LoadFacility(fac);
        }

        foreach (ProducibleFacilityData fac in data.producibleFacilities)
        {
            LoadFacility(fac);
        }
    }

    private void LoadFacility(FacilityData data)
    {
        ItemInfo info = TryGetInfo(data.type);
        this.currentItemInfo = info;
        InitializeFacility(data);
        this.facility.transform.position = data.position;
        this.facility.TryPlace();
        ContinueBuildFacilityIfNeeded(data);
    }

    private void ContinueBuildFacilityIfNeeded(FacilityData data)
    {
        if (!data.isBuilding) return;
        DateTime finishedTime = DateTime.Parse(data.finishedBuildingTime);
        this.facility.ContinueBuilding(finishedTime);
        this.facility.OnFirstTimePlaced += OnFirstTimePlacedHandler;
    }

    private ItemInfo TryGetInfo(ItemType type)
    {
        ItemInfo info = null;
        foreach (ItemInfo inf in this.itemsInfos)
        {
            if (inf.type != type) continue;
            info = inf;
            break;
        }

        if (info == null)
        {
            Debug.LogError($"Can not find info type of {type}");
        }

        return info;
    }

    private void InitializeAvailableFacilities()
    {
        foreach (Facility availableFacility in this.availableFacilities)
        {
            availableFacility.Initialize(this, this.gridLayout, this.timerTooltip, null);
            availableFacility.SetDraggable(false);
            availableFacility.SetPlaced(true);
        }
    }

    private void SpawnItemHandler(ItemInfo info)
    {
        this.currentItemInfo = info;
        InitializeFacility();
        AllowDragCurrentFacility();
        this.facility.OnFirstTimePlaced += OnFirstTimePlacedHandler;
    }

    private void InitializeFacility(FacilityData data = null)
    {
        this.facility = Instantiate(
            this.currentItemInfo.prefab, this.mainTilemap.transform);
        SetFacilityParams();
        this.facility.Initialize(this, this.gridLayout,
            this.timerTooltip, this.currentItemInfo, data);
        this.placedFacilities.Add(this.facility);
    }

    private void SetFacilityParams()
    {
        switch (this.facility)
        {
            case CropFacility cropFacility:
                cropFacility.SetCropTooltip(this.cropTooltip);
                cropFacility.SetHarvestTooltip(this.harvestTooltip);
                break;
            case ProductionFacility productionFacility:
                productionFacility.SetProduction(this.productionTooltip);
                productionFacility.SetDatabase(this.barnDatabase);
                break;
        }
    }

    private void AllowDragCurrentFacility()
    {
        this.facility.SetDraggable(true);
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

    public string GetJsonData()
    {
        BuildingSystemData data = new BuildingSystemData();
        data.facilities = new List<FacilityData>(this.placedFacilities.Count);
        data.producibleFacilities = new List<ProducibleFacilityData>(this.placedFacilities.Count);
        foreach (Facility fac in this.placedFacilities)
        {
            FacilityData d = fac.GetData();
            if (d is ProducibleFacilityData p)
            {
                data.producibleFacilities.Add(p);
            }
            else
            {
                data.facilities.Add(d);
            }
        }

        string json = JsonUtility.ToJson(data);
        return json;
    }
}