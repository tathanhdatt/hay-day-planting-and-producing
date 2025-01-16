using Dt.Attribute;
using Lean.Touch;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerTooltip : MonoBehaviour
{
    private const int numberOfGemPer10Minutes = 1;

    [SerializeField, Required]
    private RectTransform content;

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

    [Line]
    [SerializeField, Required]
    private SnappingCamera snappingCamera;

    private ICurrency currency;
    private Timer timer;

    public void Initialize(ICurrency currency)
    {
        this.currency = currency;
        this.skipButton.onClick.AddListener(OnClickSkipHandler);
        LeanTouch.OnFingerDown += OnFingerDownHandler;
    }

    private void OnFingerDownHandler(LeanFinger finger)
    {
        HideIfClickOutside(finger);
    }

    private void HideIfClickOutside(LeanFinger finger)
    {
        if (!gameObject.activeSelf) return;
        Vector3 fingerPos = finger.GetWorldPosition(CameraConstant.ZPosition);
        fingerPos = this.content.InverseTransformPoint(fingerPos);
        if (this.content.rect.Contains(fingerPos)) return;
        Hide();
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

        this.fill.fillAmount = this.timer.GetTimeLeftPercentage();
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