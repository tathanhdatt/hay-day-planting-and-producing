using System;
using Dt.Attribute;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ProducedSlotDetector : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private ProducedSlot lastProducedSlot;

    [SerializeField, ReadOnly]
    private Collider2D lastCollider2D;

    public event Action<ProducedSlot> OnDetectedSlot;

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