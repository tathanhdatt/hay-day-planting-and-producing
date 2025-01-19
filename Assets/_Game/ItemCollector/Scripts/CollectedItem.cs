using System;
using Dt.Attribute;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CollectedItem : MonoBehaviour
{
    [SerializeField, Required]
    private Image graphic;

    [SerializeField, Required]
    private TMP_Text quantityText;
    
    [SerializeField, Required]
    private AutoMover autoMover;
    
    public int Quantity { get; private set; }
    public GoodsRewardType Type { get; private set; }
    
    public event Action<CollectedItem> OnDisappear;

    public void Initialize(Sprite graphic, GoodsReward reward, Vector3 target)
    {
        this.graphic.sprite = graphic;
        Type = reward.type;
        Quantity = Random.Range(reward.quantityRange.start, reward.quantityRange.end);
        this.quantityText.SetText(Quantity.ToString());
        this.autoMover.Move(target);
        this.autoMover.OnFinished += OnFinishedHandler;
    }

    private void OnFinishedHandler()
    {
        this.autoMover.OnFinished -= OnFinishedHandler;
        OnDisappear?.Invoke(this);
    }
}