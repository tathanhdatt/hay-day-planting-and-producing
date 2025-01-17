using Dt.Attribute;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerTooltip : MonoBehaviour
{
    private const int numberOfGemPer10Minutes = 1;

    [SerializeField, Required]
    private ClickOutsideHider clickOutsideHider;

    [SerializeField, Required]
    private TMP_Text timerName;

    [SerializeField, Required]
    private TMP_Text timeLeftText;

    [SerializeField, Required]
    private TMP_Text skipPriceText;

    [SerializeField, Required]
    private TwiceClickButton skipButton;
    
    [SerializeField, Required]
    private DeltaSizeFillBar deltaSizeFillBar;

    [Line]
    [SerializeField, Required]
    private SnappingCamera snappingCamera;

    private ICurrency currency;
    private Timer timer;

    public void Initialize(ICurrency currency)
    {
        this.currency = currency;
        this.skipButton.OnConfirm += OnConfirmSkipHandler;
        this.clickOutsideHider.Initialize();
    }

    private void OnConfirmSkipHandler()
    {
        int gem = GetGemToSkip();
        if (this.currency.IsEnough(CurrencyType.Gem, gem))
        {
            this.timer?.SkipTimer();
            this.currency.SubtractAmount(CurrencyType.Gem, gem);
        }
        else
        {
            string message = $"Not enough {gem} <sprite=\"diamond\" index=0>";
            Messenger.Broadcast(Message.PopupDialog, message);
        }
    }

    public void Show(Timer timer)
    {
        if (timer == null) return;
        this.snappingCamera.SetSource(timer.transform);
        this.timer = timer;
        this.timerName.SetText(this.timer.Name);
        transform.position = this.timer.transform.position;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (this.timer == null)
        {
            Hide();
            return;
        }

        this.deltaSizeFillBar.SetFillBar(this.timer.GetTimeLeftPercentage());
        this.timeLeftText.SetText(this.timer.GetFormattedTimeLeft());
        string price = $"{GetGemToSkip().ToString()}<sprite=\"diamond\" index=0>";
        this.skipPriceText.SetText(price);
    }

    private int GetGemToSkip()
    {
        if (this.timer.TimeLeft < 600)
        {
            return numberOfGemPer10Minutes;
        }

        return (int)(this.timer.TimeLeft / 60 * numberOfGemPer10Minutes);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}