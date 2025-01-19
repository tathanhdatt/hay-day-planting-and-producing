using DG.Tweening;
using Dt.Attribute;
using UnityEngine;
using UnityEngine.UI;

public class DeltaSizeFillBar : MonoBehaviour
{
    [SerializeField, Required]
    private Image fillImage;


    [SerializeField]
    private float fillDuration;

    [SerializeField]
    private Ease ease;

    [Line]
    [SerializeField, ReadOnly]
    private float maxWidth;

    private Tweener fillTweener;

    private void Awake()
    {
        this.maxWidth = ((RectTransform)this.fillImage.transform.parent.transform).rect.width;
    }

    public async void FillBar(float percentage)
    {
        this.fillTweener?.Kill();
        Vector2 sizeDelta = GetDeltaSize();
        sizeDelta.x = percentage * this.maxWidth;
        this.fillTweener = this.fillImage.rectTransform
            .DOSizeDelta(sizeDelta, this.fillDuration);
        this.fillTweener.SetEase(this.ease);
        await this.fillTweener.AsyncWaitForCompletion();
    }

    private Vector2 GetDeltaSize()
    {
        return this.fillImage.rectTransform.sizeDelta;
    }

    public void SetFillBar(float percentage)
    {
        this.fillTweener?.Complete();
        Vector2 sizeDelta = GetDeltaSize();
        sizeDelta.x = percentage * this.maxWidth;
        this.fillImage.rectTransform.sizeDelta = sizeDelta;
    }
}