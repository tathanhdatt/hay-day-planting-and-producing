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

    [Button]
    public void UpdateRequirements()
    {
        foreach (SupplyRequirement requirement in this.requirements)
        {
            requirement.quantity += this.additionalSupplyEachLevel;
        }
    }
}