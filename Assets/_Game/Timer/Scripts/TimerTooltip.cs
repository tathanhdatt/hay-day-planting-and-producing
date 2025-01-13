using Dt.Attribute;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerTooltip : MonoBehaviour
{
    private const int numberOfGemPer10Minutes = 1;

    [SerializeField, Required]
    private TMP_Text timerName;

    [SerializeField, Required]
    private Image fill;

    [SerializeField, Required]
    private TMP_Text timeLeftText;

    [SerializeField, Required]
    private TMP_Text skipPriceText;

    [SerializeField, Required]
    private Button skipButton;

    private ICurrency currency;
    private Timer timer;
    
    public Timer CurrentTimer => this.timer;

    public void Initialize(ICurrency currency)
    {
        this.currency = currency;
        this.skipButton.onClick.AddListener(OnClickSkipHandler);
    }

    private void OnClickSkipHandler()
    {
        int gem = GetGemToSkip();
        if (this.currency.IsEnough(CurrencyType.Gem, gem))
        {
            this.timer?.SkipTimer();
            this.currency.SubtractAmount(CurrencyType.Gem, gem);
        }
        else
        {
            // TODO: Notify not enough gem
            Debug.Log($"Not enough {gem} gems");
        }
    }

    public void Show(Timer timer)
    {
        if (timer == null) return;
        gameObject.SetActive(true);
        this.timer = timer;
        this.timerName.SetText(this.timer.Name);
    }

    private void Update()
    {
        if (this.timer == null)
        {
            Hide();
            return;
        }
        transform.position = this.timer.transform.position;
        this.fill.fillAmount = this.timer.GetTimeLeftPercentage();
        this.timeLeftText.SetText(this.timer.GetFormattedTimeLeft());
        this.skipPriceText.SetText(GetGemToSkip().ToString());
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