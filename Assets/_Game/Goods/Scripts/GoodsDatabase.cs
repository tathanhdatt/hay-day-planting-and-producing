using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Goods Database", menuName = "Goods/Goods Database")]
public class GoodsDatabase : ScriptableObject
{
    public List<Goods> goods;
    public int capacity;
    public List<UpgradeInformation> requirements;

    public UpgradeInformation GetCurrentUpgradeInformation()
    {
        foreach (UpgradeInformation requirement in this.requirements)
        {
            if (requirement.IsInRangeCapacity(this.capacity))
            {
                return requirement;
            }
        }
        return null;
    }

    public int GetOccupiedSlots()
    {
        int occupied = 0;
        foreach (Goods good in this.goods)
        {
            occupied += good.quantity;
        }
        return occupied;
    }
}