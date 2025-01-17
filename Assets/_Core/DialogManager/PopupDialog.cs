using DG.Tweening;
using Dt.Attribute;
using TMPro;
using UnityEngine;

public class PopupDialog : Dialog
{
    [SerializeField, Required]
    private TMP_Text content;

    [Title("Fly Up Effect")]
    [SerializeField]
    private float flyUpDuration;

    [SerializeField]
    private Ease flyUpEase;

    [SerializeField]
    private float flyUpDistance;

    [Line]
    [SerializeField, ReadOnly]
    private Vector3 startPosition;

    private Tweener flyUpTweener;

    private void Awake()
    {
        this.startPosition = transform.position;
    }

    public override void Show()
    {
        base.Show();
        FlyUp();
    }

    private void FlyUp()
    {
        transform.position = this.startPosition;
        this.flyUpTweener?.Kill();
        this.flyUpTweener = transform.DOMoveY(
            transform.position.y + this.flyUpDistance,
            this.flyUpDuration);
        this.flyUpTweener.SetEase(this.flyUpEase);
        this.flyUpTweener.OnComplete(OnCompleteHandler);
    }

    private void OnCompleteHandler()
    {
        Hide();
    }

    public void SetContent(string content)
    {
        this.content.SetText(content);
    }
}