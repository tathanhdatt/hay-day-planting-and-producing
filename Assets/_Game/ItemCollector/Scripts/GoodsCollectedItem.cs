using System;
using Dt.Attribute;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoodsCollectedItem : MonoBehaviour
{
    [SerializeField, Required]
    private Image graphic;

    [SerializeField, Required]
    private TMP_Text quantityText;

    [SerializeField, Required]
    private AutoMover autoMover;

    [SerializeField, ReadOnly]
    private Goods goods;

    public Goods Goods => this.goods;
    public int Quantity { get; private set; }

    public event Action<GoodsCollectedItem> OnDisappear;

    public void Initialize(Sprite graphic, int quantity, Vector3 target, Goods goods)
    {
        this.graphic.sprite = graphic;
        Quantity = quantity;
        this.quantityText.SetText(Quantity.ToString());
        this.autoMover.Move(target);
        this.autoMover.OnFinished += OnFinishedHandler;
        this.goods = goods;
    }

    private void OnFinishedHandler()
    {
        this.autoMover.OnFinished -= OnFinishedHandler;
        OnDisappear?.Invoke(this);
    }
}