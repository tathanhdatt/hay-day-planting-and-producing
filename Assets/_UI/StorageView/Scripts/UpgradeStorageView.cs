using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Dt.Attribute;
using UnityEngine;
using UnityEngine.UI;

public abstract class UpgradeStorageView : BaseView
{
    [SerializeField, Required]
    private UpgradeStorageItem prefab;

    [SerializeField, Required]
    private Transform content;

    [Title("Button")]
    [SerializeField, Required]
    private Button backButton;
    
    [SerializeField, Required]
    private TwiceClickButton confirmUpgradeButton;

    [SerializeField, ReadOnly]
    private List<UpgradeStorageItem> items = new List<UpgradeStorageItem>(3);

    public event Action OnClickBack;
    public event Action OnConfirmUpgrade;
    public event Action<SupplyRequirement> OnConfirmBuyGoods;

    public override async UniTask Initialize()
    {
        await base.Initialize();
        this.backButton.onClick.AddListener(() => OnClickBack?.Invoke());
        this.confirmUpgradeButton.OnConfirm += OnConfirmUpgradeHandler;
    }

    private void OnConfirmUpgradeHandler()
    {
        OnConfirmUpgrade?.Invoke();
    }

    public void GenerateSupply(SupplyRequirement requirement)
    {
        UpgradeStorageItem item = Instantiate(this.prefab, this.content);
        item.Initialize(requirement);
        this.items.Add(item);
        item.OnConfirmBuyGoods += OnConfirmBuyGoodsHandler;
    }

    private void OnConfirmBuyGoodsHandler(SupplyRequirement requirement)
    {
        OnConfirmBuyGoods?.Invoke(requirement);
    }

    public void Refresh()
    {
        foreach (UpgradeStorageItem item in this.items)
        {
            item.Refresh();
        }
    }

    private void OnDestroy()
    {
        foreach (UpgradeStorageItem item in this.items)
        {
            item.OnConfirmBuyGoods -= OnConfirmBuyGoodsHandler;
        }
    }
}