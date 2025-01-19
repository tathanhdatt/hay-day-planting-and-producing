using System;
using Cysharp.Threading.Tasks;
using Dt.Attribute;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class StorageView : BaseView
{
    [Title("Button")]
    [SerializeField, Required]
    private Button exitButton;

    [SerializeField, Required]
    private Button upgradeButton;

    [Line]
    [SerializeField, Required]
    private GoodsHolder holder;

    [SerializeField, Required]
    private GameObject storageGameObject;

    [Line]
    [SerializeField, Required]
    private TMP_Text capacityText;

    [Title("Storage Bar")]
    [SerializeField, Required]
    private IndicatorFillBar storageFillBar;

    public event Action OnClickExit;
    public event Action OnClickUpgrade;

    public override async UniTask Initialize()
    {
        await base.Initialize();
        this.exitButton.onClick.AddListener(() => OnClickExit?.Invoke());
        this.upgradeButton.onClick.AddListener(() => OnClickUpgrade?.Invoke());
    }

    public void InitializeGoodsHolder(GoodsDatabase database)
    {
        this.holder.Initialize(database);
    }

    public void RefreshHolder()
    {
        this.holder.Refresh();
    }

    public void SetActiveStorage(bool active)
    {
        this.storageGameObject.SetActive(active);
    }

    public void SetCapacityText(string text)
    {
        this.capacityText.SetText(text);
    }

    public void MoveIndicator(float percentage)
    {
        this.storageFillBar.MoveIndicator(percentage);
    }

    public void SetIndicator(float percentage)
    {
        this.storageFillBar.SetIndicator(percentage);
    }
}