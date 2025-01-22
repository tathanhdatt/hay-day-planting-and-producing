using System;
using UnityEngine;

[Serializable]
public class GoodsReward
{
    public GoodsRewardType type;
    public Goods item;
    public IntRange quantityRange;
    [Range(0, 1)]
    public float rate;
}