using System;
using DG.Tweening;
using Dt.Attribute;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDrag : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Title("Offset")]
    [SerializeField]
    private Vector3 offset;

    [Title("Snap To Start Position")]
    [SerializeField]
    private bool snapToStart;

    [SerializeField]
    private float snappingDuration;

    [SerializeField]
    private Ease snappingEase;

    [Line]
    [SerializeField, ReadOnly]
    private Vector3 startPosition;

    [SerializeField, ReadOnly]
    private Camera cam;

    private Tweener snappingTweener;
    
    public event Action OnFingerDown;
    public event Action OnFingerUp;

    private void Awake()
    {
        if (this.cam == null)
        {
            this.cam = Camera.main;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnFingerDown?.Invoke();
        SaveStartPosition();
        transform.SetAsLastSibling();
    }

    private void SaveStartPosition()
    {
        if (this.snapToStart)
        {
            this.startPosition = transform.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 pos = this.cam.ScreenToWorldPoint(eventData.position);
        pos.z = 0;
        transform.position = pos;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnFingerUp?.Invoke();
        if (this.snapToStart)
        {
            SnapToStartPosition();
        }
    }

    private void SnapToStartPosition()
    {
        this.snappingTweener?.Kill(true);
        this.snappingTweener = transform.DOMove(this.startPosition, this.snappingDuration);
        this.snappingTweener.SetEase(this.snappingEase);
    }
}