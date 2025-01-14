using System.Collections.Generic;
using Dt.Attribute;
using UnityEngine;

[CreateAssetMenu(fileName = "New Requirement", menuName = "Goods/Upgrade/Upgrade Requirement")]
public class UpgradeInformation : ScriptableObject
{
    public int fromCapacity;
    public int toCapacity;
    public int additionalCapacityEachLevel;
    public int additionalSupplyEachLevel;
    public List<SupplyRequirement> requirements;

    public bool IsInRangeCapacity(int capacity)
    {
        if (this.fromCapacity <= capacity && capacity <= this.toCapacity)
        {
            return true;
        }
        return false;
    }

    public bool CanUpgrade()
    {
        foreach (SupplyRequirement requirement in this.requirements)
        {
            if (requirement.requiredQuantity > requirement.goods.quantity)
            {
                return false;
            }
        }

        return true;
    }
}