using Dt.Attribute;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoodsItem : MonoBehaviour
{
    [SerializeField, Required]
    private Image icon;
    
    [SerializeField, Required]
    private TMP_Text quantity;
    
    [SerializeField, ReadOnly]
    private Goods goods;

    public void Initialize(Goods goods)
    {
        this.goods = goods;
        Refresh();
    }

    public void Refresh()
    {
        this.icon.sprite = this.goods.graphic;
        this.quantity.SetText(this.goods.quantity.ToString());
    }
}