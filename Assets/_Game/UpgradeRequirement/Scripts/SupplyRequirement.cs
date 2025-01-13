using UnityEngine;

[CreateAssetMenu(fileName = "New Supply Requirement", menuName = "Goods/Upgrade/Goods Requirement")]
public class SupplyRequirement : ScriptableObject
{
    public int quantity;
    public Goods goods;
}