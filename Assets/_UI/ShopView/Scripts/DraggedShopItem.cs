using DG.Tweening;
using Dt.Attribute;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggedShopItem : MonoBehaviour,
    IDragHandler, IEndDragHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [SerializeField, Required]
    private Canvas iconCanvas;

    [SerializeField, ReadOnly]
    private RectTransform draggingIcon;

    [SerializeField, ReadOnly]
    private bool isDragging;

    [SerializeField, ReadOnly]
    private bool isOutOfShop;

    [SerializeField, ReadOnly]
    private Vector2 startPosition;

    [SerializeField, ReadOnly]
    private Transform draggingBound;

    private Tweener punchIconTweener;
    private ShopItemInfo info;

    public void Initialize(Transform draggingBound, RectTransform draggingIcon, ShopItemInfo info)
    {
        this.draggingBound = draggingBound;
        this.draggingIcon = draggingIcon;
        this.startPosition = this.draggingIcon.anchoredPosition;
        this.info = info;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!this.info.isEnoughCurrency) return;
        if (this.isOutOfShop) return;
        if (Camera.main is null) return;
        SetInteractableLayerForItemIcon();
        Vector3 position = Camera.main.ScreenToWorldPoint(eventData.position);
        position.z = 0;
        this.draggingIcon.position = position;
        this.isOutOfShop = position.x >= this.draggingBound.position.x;
        if (this.isOutOfShop)
        {
            HandleOnOutOfShop();
        }
    }

    private void SetInteractableLayerForItemIcon()
    {
        this.iconCanvas.overrideSorting = true;
        this.iconCanvas.sortingLayerName = "Interactable";
    }

    private void HandleOnOutOfShop()
    {
        Messenger.Broadcast(Message.OutOfShopBound);
        Messenger.Broadcast(Message.SpawnItem, this.info);
        MoveIconToStartPosition();
        ResetLayerForItemIcon();
    }

    private void ResetLayerForItemIcon()
    {
        this.iconCanvas.overrideSorting = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.iconCanvas.overrideSorting = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.punchIconTweener?.Complete();
        this.punchIconTweener = this.draggingIcon
            .DOPunchScale(this.draggingIcon.localScale * 1.02f, 0.2f);
        if (this.info.isEnoughCurrency) return;
        // TODO: Notify not enough coin or gem
        Debug.Log($"Not enough {this.info.currencyType}");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (this.isOutOfShop) return;
        MoveIconToStartPosition();
    }

    private void MoveIconToStartPosition()
    {
        this.draggingIcon.DOAnchorPos(this.startPosition, 0.2f);
    }

    private void OnDisable()
    {
        this.isOutOfShop = false;
    }
}