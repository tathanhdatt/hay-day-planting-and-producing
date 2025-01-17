using System;
using Dt.Attribute;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ProducedSlotDetector : MonoBehaviour
{
    [SerializeField, Required]
    private UIDrag drag;

    [SerializeField, Required]
    private Collider2D detectSlotCollider;

    [SerializeField, ReadOnly]
    private ProducedSlot lastProducedSlot;

    [SerializeField, ReadOnly]
    private Collider2D lastCollider2D;
    
    public event Action<ProducedSlot> OnDetectedSlot;

    public void Initialize()
    {
        this.detectSlotCollider.enabled = false;
        this.drag.OnFingerDown += OnFingerDownHandler;
        this.drag.OnFingerUp += OnFingerUpHandler;
    }

    private void OnFingerDownHandler()
    {
        this.detectSlotCollider.enabled = true;
    }

    private void OnFingerUpHandler()
    {
        this.detectSlotCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        bool isNewCollider = this.lastCollider2D != other;
        if (isNewCollider)
        {
            ProducedSlot producedSlot = other.GetComponent<ProducedSlot>();
            if (producedSlot == null) return;
            this.lastProducedSlot = producedSlot;
        }

        OnDetectedSlot?.Invoke(this.lastProducedSlot);
    }
}