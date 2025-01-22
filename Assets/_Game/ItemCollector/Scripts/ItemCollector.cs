using System;
using System.Collections.Generic;
using Core.Service;
using Cysharp.Threading.Tasks;
using Dt.Attribute;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ItemCollector : MonoBehaviour
{
    [SerializeField, Required]
    private ProducedSlotCleaner cleaner;

    [Line]
    [SerializeField, Required]
    private CollectedItem prefab;
    
    [Title("Xp")]
    [SerializeField, Required]
    private Sprite xpGraphic;

    [SerializeField, Required]
    private Transform xpTarget;

    [Title("Coin")]
    [SerializeField, Required]
    private Sprite coinGraphic;

    [SerializeField, Required]
    private Transform coinTarget;

    [Title("Diamond")]
    [SerializeField, Required]
    private Sprite diamondGraphic;

    [SerializeField, Required]
    private Transform diamondTarget;

    [Title("Goods")]
    [SerializeField, Required]
    private GoodsCollectedItem goodsPrefab;

    [SerializeField, Required]
    private Transform goodsTarget;

    [Title("Database")]
    [SerializeField, ReadOnly]
    private GoodsDatabase siloDatabase;

    [SerializeField, ReadOnly]
    private GoodsDatabase barnDatabase;

    [Title("Visual")]
    [SerializeField, Required]
    private IndicatorFillBar indicatorFillBar;

    [SerializeField, Required]
    private TMP_Text capacityText;

    [SerializeField, Required]
    private Image icon;

    [Line]
    [SerializeField, Required]
    private Sprite siloIcon;

    [SerializeField, Required]
    private Sprite barnIcon;

    private ILevelXpStorage levelXpStorage;
    private ICurrency currency;

    public void Initialize(ILevelXpStorage levelXpStorage, ICurrency currency,
        GoodsDatabase siloDatabase, GoodsDatabase barnDatabase)
    {
        this.levelXpStorage = levelXpStorage;
        this.currency = currency;
        this.siloDatabase = siloDatabase;
        this.barnDatabase = barnDatabase;
        this.cleaner.OnCleaned += OnCleanedHandler;
        Messenger.AddListener<Goods, int, Vector3>(
            Message.CollectGoods, OnCollectGoodsHandler);
        Messenger.AddListener<List<GoodsReward>, Vector3>(
            Message.CollectRewards, OnCollectRewardsHandler);
    }

    private void OnCleanedHandler(ProducedSlot slot)
    {
        OnCollectGoodsHandler(slot.Recipe.product, ExchangeRate.CropPerSeed,
            slot.transform.position);
        OnCollectRewardsHandler(slot.Recipe.rewards, slot.transform.position);
    }

    private void OnCollectGoodsHandler(Goods goods, int quantity, Vector3 position)
    {
        goods.quantity += quantity;
        GoodsCollectedItem collectedItem = GetCollectedItem(this.goodsPrefab, position);
        collectedItem.Initialize(goods.graphic, quantity, this.goodsTarget.position, goods);
        ShowDatabase(collectedItem);
        collectedItem.OnDisappear += OnGoodsDisappearHandler;
    }

    private void ShowDatabase(GoodsCollectedItem item)
    {
        bool isCrop = this.siloDatabase.goods.Contains(item.Goods);
        if (isCrop)
        {
            this.icon.sprite = this.siloIcon;
            ShowDatabase(this.siloDatabase);
        }
        else
        {
            this.icon.sprite = this.barnIcon;
            ShowDatabase(this.barnDatabase);
        }
    }

    private void OnGoodsDisappearHandler(GoodsCollectedItem item)
    {
        item.OnDisappear -= OnGoodsDisappearHandler;
        ServiceLocator.GetService<IPoolService>().Despawn(item);
    }

    private UniTask currentTask;

    private async void ShowDatabase(GoodsDatabase database)
    {
        float storagePercentage = database.GetOccupiedSlots() / (float)database.capacity;
        this.indicatorFillBar.transform.parent.gameObject.SetActive(true);
        this.indicatorFillBar.MoveIndicator(storagePercentage);
        this.capacityText.SetText($"{database.GetOccupiedSlots()}/{database.capacity}");
        this.currentTask = UniTask.WaitForSeconds(2);
        await this.currentTask;
        this.indicatorFillBar.transform.parent.gameObject.SetActive(false);
    }

    private async void OnCollectRewardsHandler(List<GoodsReward> rewards, Vector3 startPosition)
    {
        foreach (GoodsReward reward in rewards)
        {
            if (reward.rate < Random.Range(0f, 1f)) continue;
            startPosition.x += 1;
            OnCollectedHandler(reward, startPosition);
            await UniTask.WaitForSeconds(0.2f);
        }
    }

    private void OnCollectedHandler(GoodsReward reward, Vector3 startPosition)
    {
        CollectedItem collectedItem = GetCollectedItem(this.prefab, startPosition);
        Vector3 position = reward.type switch
        {
            GoodsRewardType.Coin => this.coinTarget.position,
            GoodsRewardType.Gem => this.diamondTarget.position,
            GoodsRewardType.Xp => this.xpTarget.position,
            GoodsRewardType.Supply => this.goodsTarget.position,
            _ => Vector3.zero
        };
        Sprite graphic = reward.type switch
        {
            GoodsRewardType.Coin => this.coinGraphic,
            GoodsRewardType.Gem => this.diamondGraphic,
            GoodsRewardType.Xp => this.xpGraphic,
            GoodsRewardType.Supply => reward.item.graphic,
            _ => null
        };
        collectedItem.Initialize(graphic, reward, position);
        collectedItem.OnDisappear += OnItemDisappearHandler;
    }

    private T GetCollectedItem<T>(T prefab, Vector3 position) where T : MonoBehaviour
    {
        T collectedItem = ServiceLocator
            .GetService<IPoolService>().Spawn(prefab, transform);
        collectedItem.transform.position = position;
        collectedItem.gameObject.SetActive(true);
        return collectedItem;
    }

    private void OnItemDisappearHandler(CollectedItem collectedItem)
    {
        collectedItem.OnDisappear -= OnItemDisappearHandler;
        ServiceLocator.GetService<IPoolService>().Despawn(collectedItem);
        switch (collectedItem.Type)
        {
            case GoodsRewardType.Xp:
                this.levelXpStorage.AddXp(collectedItem.Quantity);
                break;
            case GoodsRewardType.Coin:
                this.currency.AddAmount(CurrencyType.Coin, collectedItem.Quantity);
                break;
            case GoodsRewardType.Gem:
                this.currency.AddAmount(CurrencyType.Gem, collectedItem.Quantity);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}