using System;
using Dt.Attribute;
using UnityEngine;

[RequireComponent(typeof(ProducedSlotDetector))]
public class ProducedSlotCleaner : MonoBehaviour
{
    [SerializeField, Required]
    private ProducedSlotDetector detector;

    public event Action OnCleaned;

    public void Initialize()
    {
        this.detector.Initialize();
        this.detector.OnDetectedSlot += OnDetectedSlotHandler;
    }

    private void OnDetectedSlotHandler(ProducedSlot slot)
    {
        slot.FreeSlot();
        OnCleaned?.Invoke();
    }
}