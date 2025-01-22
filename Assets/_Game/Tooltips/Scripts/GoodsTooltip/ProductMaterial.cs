using Core.Service;
using Dt.Attribute;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProductMaterial : MonoBehaviour
{
    [SerializeField, Required]
    private TMP_Text quantity;

    [SerializeField, Required]
    private Image icon;

    private void Awake()
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void Show(int quantity, Sprite icon)
    {
        this.quantity.SetText(quantity.ToString());
        this.icon.sprite = icon;
        gameObject.SetActive(true);
    }

    public void Despawn()
    {
        ServiceLocator.GetService<IPoolService>().Despawn(this);
    }
}