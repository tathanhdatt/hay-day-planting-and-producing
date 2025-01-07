using Dt.Attribute;
using Unity.VisualScripting;
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

    [SerializeField, Required]
    private TileBase hoveringTile;

    private DraggableObject currentDraggableObject;

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
        PlaceableObject newItem = Instantiate(info.prefab, this.mainTilemap.transform);
        this.currentDraggableObject = newItem.AddComponent<DraggableObject>();
        newItem.Initialize(this.currentDraggableObject);
        this.currentDraggableObject.Initialize(this, this.gridLayout, newItem.Bounds);
        this.currentDraggableObject.OnPlacedNewItem += OnPlacedNewItem;
    }

    private void OnPlacedNewItem()
    {
        this.currentDraggableObject.OnPlacedNewItem -= OnPlacedNewItem;
        SubtractAmount();
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

    public void HoverTilesInArea(BoundsInt area)
    {
        SetTilesInArea(area, this.hoveringTile);
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