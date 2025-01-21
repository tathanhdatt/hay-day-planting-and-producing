using System.Collections.Generic;
using Dt.Attribute;
using Lean.Touch;
using UnityEngine;
using UnityEngine.EventSystems;

public class CropTooltip : MonoBehaviour
{
    [SerializeField, Required]
    private SnappingCamera snappingCamera;

    [SerializeField, Required]
    private ClickOutsideHider clickOutsideHider;

    [SerializeField, Required]
    private RectTransform content;

    [SerializeField]
    private List<ProducedSlotFiller> producedSlotFillers;

    [SerializeField, ReadOnly]
    private List<GoodsRecipe> recipes;

    [SerializeField, ReadOnly]
    private Transform lastSibling;

    public void Initialize()
    {
        EventSystem.current.SetSelectedGameObject(gameObject, null);
        LeanTouch.OnFingerUp += OnFingerUpHandler;
        this.clickOutsideHider.Initialize();
        InitializeProducibleGoods();
    }

    private void InitializeProducibleGoods()
    {
        foreach (ProducedSlotFiller goods in this.producedSlotFillers)
        {
            goods.Initialize();
        }
    }

    private void OnFingerUpHandler(LeanFinger finger)
    {
        if (this.content.gameObject.activeSelf) return;
        Hide();
        ResetContent();
    }

    public void Show(List<GoodsRecipe> recipes, Transform source)
    {
        ResetContent();
        this.snappingCamera.SetSource(source);
        this.recipes = recipes;
        FillSlots();
        gameObject.SetActive(true);
    }

    private void FillSlots()
    {
        int numberOfRecipes = this.recipes.Count;
        for (int i = 0; i < this.producedSlotFillers.Count; i++)
        {
            if (i < numberOfRecipes)
            {
                this.producedSlotFillers[i].Show(this.recipes[i]);
                this.producedSlotFillers[i].gameObject.SetActive(true);
            }
            else
            {
                this.producedSlotFillers[i].gameObject.SetActive(false);
            }
        }
    }

    public void HideContent()
    {
        if (this.lastSibling == null)
        {
            SetSiblingOutOfContent();
            DisableContent();
            return;
        }

        bool isLastSiblingInContent = this.lastSibling.IsChildOf(this.content);
        if (!isLastSiblingInContent) return;
        SetSiblingOutOfContent();
        DisableContent();
    }

    private void SetSiblingOutOfContent()
    {
        this.lastSibling = this.content.GetChild(this.content.childCount - 1);
        this.lastSibling.SetParent(transform);
    }

    private void DisableContent()
    {
        this.content.gameObject.SetActive(false);
    }

    public void RefreshContent()
    {
        foreach (ProducedSlotFiller slotFiller in this.producedSlotFillers)
        {
            if (slotFiller.gameObject.activeSelf)
            {
                slotFiller.Refresh();
            }
        }
    }

    public void ResetContent()
    {
        this.content.gameObject.SetActive(true);
        if (this.lastSibling == null) return;
        this.lastSibling.SetParent(this.content);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}