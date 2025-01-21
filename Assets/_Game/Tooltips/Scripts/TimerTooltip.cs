using Dt.Attribute;
using TMPro;
using UnityEngine;

public class TimerTooltip : MonoBehaviour
{

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
        int gem = this.timer.GetGemToSkip();
        if (this.currency.IsEnough(CurrencyType.Gem, gem))
        {
            this.timer.SkipTimer();
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
        string price = $"{this.timer.GetGemToSkip().ToString()}<sprite=\"diamond\" index=0>";
        this.skipPriceText.SetText(price);
    }

    

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}