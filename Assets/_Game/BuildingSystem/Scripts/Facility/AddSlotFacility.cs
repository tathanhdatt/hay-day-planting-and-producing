using UnityEngine;

public class AddSlotFacility : Facility
{
    [SerializeField]
    private StorageSlotType type;

    public StorageSlotType Type => this.type;

    public void AddSlot()
    {
        OnFirstTimePlacedHandler();
    }

    public override FacilityData GetData()
    {
        return null;
    }
}