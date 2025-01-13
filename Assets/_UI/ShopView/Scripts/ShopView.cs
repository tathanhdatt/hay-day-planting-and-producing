using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Dt.Attribute;
using UnityEngine;
using UnityEngine.UI;

public class ShopView : BaseView
{
    [Title("Bound")]
    [SerializeField, Required]
    private Transform draggingBound;

    [Title("Tabs")]
    [SerializeField]
    private TabToggle[] tabButtons;

    [SerializeField]
    private ShopItemHolder[] itemHolders;

    [Title("Buttons")]
    [SerializeField, Required]
    private Button exitButton;

    [Title("Panel")]
    [SerializeField, Required]
    private RectTransform panelTransform;

    [SerializeField]
    private float panelBeginXPosition;

    [SerializeField]
    private float panelEndXPosition;

    [SerializeField]
    private Ease slidePanelEase;

    [SerializeField]
    private float slideDuration;

    public event Action OnClickExit;
    
    public ShopItemHolder[] ItemHolders => this.itemHolders;
    
    public override async UniTask Initialize()
    {
        await base.Initialize();
        this.exitButton.onClick.AddListener(() => OnClickExit?.Invoke());
        await InitializeItemHolders();
        await InitializeTabButtons();
        ResetPanelPosition();
    }

    private async UniTask InitializeItemHolders()
    {
        foreach (ShopItemHolder itemHolder in this.itemHolders)
        {
            await itemHolder.Initialize(this.draggingBound);
            itemHolder.gameObject.SetActive(false);
        }
    }

    public void RefreshHolders()
    {
        foreach (ShopItemHolder holder in this.itemHolders)
        {
            holder.Refresh();
        }
    }

    public void OnUpdateLevel()
    {
        foreach (ShopItemHolder holder in this.itemHolders)
        {
            holder.OnUpdateLevel();
        }
    }

    private async UniTask InitializeTabButtons()
    {
        foreach (TabToggle button in this.tabButtons)
        {
            await button.Initialize();
            button.OnTurnOn += OnTurnOnHandler;
        }
    }

    private void OnTurnOnHandler(TabToggle toggle)
    {
        foreach (ShopItemHolder holder in this.itemHolders)
        {
            bool isCorrectTab = holder.ItemType == toggle.Type;
            holder.gameObject.SetActive(isCorrectTab);
        }

        TurnOffOtherTabButtons(toggle);
    }

    private void TurnOffOtherTabButtons(TabToggle toggle)
    {
        foreach (TabToggle tabButton in this.tabButtons)
        {
            if (tabButton == toggle) continue;
            tabButton.TurnOff();
        }
    }

    public override async UniTask Show()
    {
        ResetPanelPosition();
        this.tabButtons[0].TurnOn();
        await base.Show();
        await SlidePanelFromLeft();
    }

    public override async UniTask Hide()
    {
        await SlidePanelToBeginPosition();
        await base.Hide();
    }

    private void ResetPanelPosition()
    {
        Vector2 localPosition = this.panelTransform.anchoredPosition;
        localPosition.x = this.panelBeginXPosition;
        this.panelTransform.anchoredPosition = localPosition;
    }

    private async UniTask SlidePanelFromLeft()
    {
        Tweener slidePanelTweener = this.panelTransform
            .DOAnchorPosX(this.panelEndXPosition, this.slideDuration);
        slidePanelTweener.SetEase(this.slidePanelEase);
        await slidePanelTweener.AsyncWaitForCompletion();
    }

    private async UniTask SlidePanelToBeginPosition()
    {
        Tweener slidePanelTweener = this.panelTransform
            .DOAnchorPosX(this.panelBeginXPosition, this.slideDuration / 2f);
        slidePanelTweener.SetEase(this.slidePanelEase);
        await slidePanelTweener.AsyncWaitForCompletion();
    }
}