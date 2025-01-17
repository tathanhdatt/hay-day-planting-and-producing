using Dt.Attribute;
using UnityEngine;

[RequireComponent(typeof(ProducedSlotDetector))]
public class ProducedSlotCleaner : MonoBehaviour
{
    [SerializeField, Required]
    private ProducedSlotDetector detector;

    public void Initialize()
    {
        this.detector.Initialize();
        this.detector.OnDetectedSlot += OnDetectedSlotHandler;
    }

    private void OnDetectedSlotHandler(ProducedSlot slot)
    {
        slot.FreeSlot();
    }
}