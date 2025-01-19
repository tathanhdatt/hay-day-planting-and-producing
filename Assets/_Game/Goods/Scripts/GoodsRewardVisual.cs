using UnityEngine;

[CreateAssetMenu(fileName = "New Goods Reward Visual", menuName = "New Goods Reward Visual")]
public class GoodsRewardVisual : ScriptableObject
{
    public Sprite graphic;
    public GoodsRewardType goodsRewardType;
}