using UnityEngine;

[CreateAssetMenu(fileName = "New Goods", menuName = "Goods/Goods")]
public class Goods : ScriptableObject
{
    public string goodsName;
    public Sprite graphic;
    public int quantity;
    public int level;
}