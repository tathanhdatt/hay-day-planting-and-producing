using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
    private Transform indicator;

    [SerializeField, Required]
    private Transform leftIndicatorBound;

    [SerializeField, Required]
    private Transform rightIndicatorBound;

    [SerializeField]
    private float indicatorMoveDuration;

    [SerializeField]
    private Ease indicatorMoveEase;

    private Tweener indicatorMoveTweener;

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
        this.indicatorMoveTweener?.Kill(true);
        float indicatorPositionX = GetIndicatorX(percentage);
        this.indicatorMoveTweener =
            this.indicator.DOLocalMoveX(indicatorPositionX, this.indicatorMoveDuration);
        this.indicatorMoveTweener.SetEase(this.indicatorMoveEase);
    }

    public void SetIndicator(float percentage)
    {
        float x = GetIndicatorX(percentage);
        Vector3 position = this.indicator.localPosition;
        position.x = x;
        this.indicator.localPosition = position;
    }

    private float GetIndicatorX(float percentage)
    {
        percentage = Mathf.Clamp01(percentage);
        return Mathf.Lerp(
            this.leftIndicatorBound.localPosition.x,
            this.rightIndicatorBound.localPosition.x,
            percentage);
    }
}