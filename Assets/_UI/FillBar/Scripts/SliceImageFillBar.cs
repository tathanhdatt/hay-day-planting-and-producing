using DG.Tweening;
using Dt.Attribute;
using UnityEngine;
using UnityEngine.UI;

public class SliceImageFillBar : MonoBehaviour
{
    [SerializeField, Required]
    private Image fillImage;

    [SerializeField]
    private float fillDuration;

    [SerializeField]
    private Ease ease;

    private Tweener fillTweener;

    public async void FillTo(float percentage)
    {
        this.fillTweener?.Complete();
        this.fillTweener = this.fillImage.DOFillAmount(percentage, this.fillDuration);
        this.fillTweener.SetEase(this.ease);
        await this.fillTweener.AsyncWaitForCompletion();
    }

    public void SetFillAmount(float percentage)
    {
        this.fillTweener?.Complete();
        this.fillImage.fillAmount = percentage;
    }
}