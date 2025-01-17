using System;
using DG.Tweening;
using Dt.Attribute;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TwiceClickButton : MonoBehaviour, IDeselectHandler
{
    private const string clickAgainMessage = "Click again to confirm!";

    [SerializeField, Required]
    private Button button;

    [SerializeField, ReadOnly]
    private bool isClicked;

    private Tweener scaleTweener;

    public event Action OnClick;
    public event Action OnConfirm;

    private void Awake()
    {
        this.isClicked = false;
        this.button.onClick.AddListener(OnClickHandler);
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ResetState();
    }

    private void OnEnable()
    {
        ResetState();
    }

    private void OnDisable()
    {
        this.isClicked = false;
    }

    private void ResetState()
    {
        this.scaleTweener?.Kill(true);
        this.isClicked = false;
    }

    private void OnClickHandler()
    {
        if (this.isClicked)
        {
            this.scaleTweener?.Kill(true);
            OnClick?.Invoke();
            OnConfirm?.Invoke();
        }
        else
        {
            OnClick?.Invoke();
            this.isClicked = true;
            Messenger.Broadcast(Message.PopupDialog, clickAgainMessage);
            ClickedScaleEffect();
        }
    }

    private void ClickedScaleEffect()
    {
        this.scaleTweener = transform.DOScale(Vector3.one * 1.1f, 0.4f);
        this.scaleTweener.SetLoops(-1, LoopType.Yoyo);
        this.scaleTweener.SetEase(Ease.InQuart);
    }
}