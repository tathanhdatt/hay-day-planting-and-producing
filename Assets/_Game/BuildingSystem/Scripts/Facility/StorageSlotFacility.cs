using UnityEngine;

public class StorageSlotFacility : Facility
{
    [SerializeField]
    private ProducedSlot prefab;

    [SerializeField]
    private StorageSlotType type;

    public StorageSlotType Type => this.type;

    private void OnTriggerEnter(Collider other)
    {
        AddSlotFacility slot = other.GetComponent<AddSlotFacility>();
        slot.AddSlot();
    }
}