using System.Collections.Generic;
using Core.Service;
using Dt.Attribute;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ProducedSlotDetector))]
public class ProducedSlotFiller : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField, Required]
    private ProductMaterial prefab;

    [SerializeField, Required]
    private Transform materialsParent;

    [SerializeField, Required]
    private ProducedSlotDetector detector;

    [SerializeField, Required]
    private GoodsItem goodsItem;

    [SerializeField, ReadOnly]
    private GoodsRecipe recipe;
    
    private List<ProductMaterial> materials = new List<ProductMaterial>(3);

    public void Initialize()
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
        this.detector.Initialize();
        this.detector.OnDetectedSlot += OnDetectedSlotHandler;
    }

    public void Show(GoodsRecipe recipe)
    {
        this.recipe = recipe;
        this.goodsItem.Initialize(this.recipe.product);
        Refresh();
    }

    private void OnDetectedSlotHandler(ProducedSlot slot)
    {
        slot.AddRecipe(this.recipe);
    }

    public void Refresh()
    {
        this.goodsItem.Refresh();
    }

    private void OnDisable()
    {
        this.materialsParent.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.materialsParent.gameObject.activeSelf) return;
        this.materialsParent.gameObject.SetActive(true);
        foreach (GoodsRequirement requirement in this.recipe.materials)
        {
            ProductMaterial material =
                ServiceLocator.GetService<IPoolService>().Spawn(this.prefab, this.materialsParent);
            material.Show(requirement.requiredQuantity, requirement.goods.graphic);
            this.materials.Add(material);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        this.materialsParent.gameObject.SetActive(false);
        foreach (ProductMaterial material in this.materials)
        {
            material.Despawn();
        }
    }
}