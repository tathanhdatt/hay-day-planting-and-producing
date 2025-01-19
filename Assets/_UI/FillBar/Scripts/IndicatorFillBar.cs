using DG.Tweening;
using Dt.Attribute;
using UnityEngine;

public class IndicatorFillBar : MonoBehaviour
{
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