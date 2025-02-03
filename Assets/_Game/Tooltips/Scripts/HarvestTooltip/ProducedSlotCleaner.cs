using System;
using Dt.Attribute;
using UnityEngine;

[RequireComponent(typeof(ProducedSlotDetector))]
public class ProducedSlotCleaner : MonoBehaviour
{
    [SerializeField, Required]
    private ProducedSlotDetector detector;

    [SerializeField, Required]
    private GoodsDatabase goodsDatabase;

    public event Action<ProducedSlot> OnCleaned;

    public void Initialize()
    {
        this.detector.OnDetectedSlot += OnDetectedSlotHandler;
    }

    private void OnDetectedSlotHandler(ProducedSlot slot)
    {
        if (!slot.CanFreeSlot()) return;
        bool isFull = this.goodsDatabase.GetOccupiedSlots() >= this.goodsDatabase.capacity;
        if (isFull)
        {
            Messenger.Broadcast(Message.PopupDialog, "Silo is full!");
        }
        else
        {
            slot.FreeSlot();
            OnCleaned?.Invoke(slot);
        }
    }
}