using System;
using System.Collections.Generic;
using Dt.Attribute;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProductionTooltip : MonoBehaviour
{
    public const int MaxSlot = 5;

    [SerializeField, Required]
    private SnappingCamera snappingCamera;

    [SerializeField, Required]
    private ClickOutsideHider clickOutsideHider;

    [Title("Buttons")]
    [SerializeField, Required]
    private TwiceClickButton addSlotButton;

    [SerializeField, Required]
    private TwiceClickButton skipButton;

    [Line]
    [SerializeField]
    private List<ProducedSlotFiller> producedSlotFillers;

    [SerializeField]
    private List<ProducedSlot> slots;

    [SerializeField]
    private List<Image> producingIconHolders;

    [Title("Timer bar")]
    [SerializeField, Required]
    private DeltaSizeFillBar timerBar;

    [SerializeField, Required]
    private TMP_Text timeLeftText;

    [SerializeField, Required]
    private TMP_Text gemSkipText;

    [Title("Read Only Fields")]
    [SerializeField, ReadOnly]
    private List<GoodsRecipe> recipes;

    private Timer currentTimer;
    private List<Sprite> producingIcons;
    private ItemInfo currentInfo;

    public event Action<GoodsRecipe> OnAddRecipe;
    public event Action OnHidden;

    private ICurrency currency;

    public void Initialize(ICurrency currency)
    {
        this.currency = currency;
        EventSystem.current.SetSelectedGameObject(gameObject, null);
        this.clickOutsideHider.Initialize();
        InitializeProducibleGoods();
        InitializeSlots();
        this.addSlotButton.OnConfirm += OnClickAddSlotHandler;
        this.skipButton.OnConfirm += OnClickSkipHandler;
    }

    private void InitializeSlots()
    {
        foreach (ProducedSlot slot in this.slots)
        {
            slot.OnAddRecipe += OnAddRecipeHandler;
        }
    }

    private void OnAddRecipeHandler(GoodsRecipe recipe)
    {
        OnAddRecipe?.Invoke(recipe);
    }

    private void InitializeProducibleGoods()
    {
        foreach (ProducedSlotFiller goods in this.producedSlotFillers)
        {
            goods.Initialize();
        }
    }

    private void OnClickAddSlotHandler()
    {
        if (this.currency.IsEnough(CurrencyType.Gem, ExchangeRate.GemPerProductionSlot))
        {
            int slot = this.currentInfo.numberOfSlot + 1;
            slot = Math.Clamp(slot, 1, MaxSlot);
            this.currentInfo.numberOfSlot = slot;
            UpdateSlots();
        }
        else
        {
            string message = $"Not enough {ExchangeRate.GemPerProductionSlot} <sprite=\"diamond\" index=0>";
            Messenger.Broadcast(Message.PopupDialog, message);
        }
    }

    private void OnClickSkipHandler()
    {
        int gem = this.currentTimer.GetGemToSkip();
        if (this.currency.IsEnough(CurrencyType.Gem, gem))
        {
            this.currentTimer.SkipTimer();
            this.currency.SubtractAmount(CurrencyType.Gem, gem);
        }
        else
        {
            string message = $"Not enough {gem} <sprite=\"diamond\" index=0>";
            Messenger.Broadcast(Message.PopupDialog, message);
        }
    }

    public void Show(List<GoodsRecipe> recipes,
        Transform source, Timer producingTimer,
        List<Sprite> producingIcons, ItemInfo itemInfo)
    {
        this.recipes = recipes;
        this.snappingCamera.SetSource(source);
        this.producingIcons = producingIcons;
        this.currentInfo = itemInfo;
        ShowTimer(producingTimer);
        FillRecipes();
        UpdateSlots();
        FillSlots();
        gameObject.SetActive(true);
    }

    private void ShowTimer(Timer producingTimer)
    {
        this.currentTimer = producingTimer;
        this.currentTimer.OnStarted += OnTimerStartHandler;
        SetTimerActive(!this.currentTimer.IsFinished);
    }

    private void OnTimerStartHandler()
    {
        SetTimerActive(true);
    }

    private void SetTimerActive(bool active)
    {
        this.timerBar.gameObject.SetActive(active);
        this.timeLeftText.gameObject.SetActive(active);
        this.skipButton.gameObject.SetActive(active);
    }

    private void FillRecipes()
    {
        int numberOfRecipes = this.recipes.Count;
        for (int i = 0; i < this.producedSlotFillers.Count; i++)
        {
            if (i < numberOfRecipes)
            {
                this.producedSlotFillers[i].Show(this.recipes[i]);
                this.producedSlotFillers[i].gameObject.SetActive(true);
            }
            else
            {
                this.producedSlotFillers[i].gameObject.SetActive(false);
            }
        }
    }

    private void UpdateSlots()
    {
        if (this.currentInfo.numberOfSlot == MaxSlot)
        {
            this.addSlotButton.gameObject.SetActive(false);
        }
        else
        {
            this.addSlotButton.gameObject.SetActive(true);
        }

        for (int i = 0; i < this.slots.Count; i++)
        {
            bool isOpened = i < this.currentInfo.numberOfSlot;
            this.slots[i].gameObject.SetActive(isOpened);
        }
    }

    private void FillSlots()
    {
        for (int i = 0; i < this.producingIconHolders.Count; i++)
        {
            bool hasIcon = i < this.producingIcons.Count;
            if (hasIcon)
            {
                this.producingIconHolders[i].sprite = this.producingIcons[i];
                this.producingIconHolders[i].gameObject.SetActive(true);
            }
            else
            {
                this.producingIconHolders[i].sprite = null;
                this.producingIconHolders[i].gameObject.SetActive(false);
                this.slots[i].FreeSlot();
            }
        }
    }

    private void Update()
    {
        if (this.currentTimer.IsFinished) return;
        this.timerBar.SetFillBar(this.currentTimer.GetTimeLeftPercentage());
        this.timeLeftText.SetText(this.currentTimer.GetFormattedTimeLeft());
        string skipGem = $"{this.currentTimer.GetGemToSkip()} <sprite=\"diamond\" index=0>";
        this.gemSkipText.SetText(skipGem);
    }

    public void Refresh()
    {
        FillSlots();
        DisableTimerIfNoProducts();
    }

    private void DisableTimerIfNoProducts()
    {
        if (this.producingIcons.IsEmpty())
        {
            SetTimerActive(false);
        }
    }

    private void OnDisable()
    {
        this.currentTimer.OnStarted -= OnTimerStartHandler;
        OnHidden?.Invoke();
    }
}