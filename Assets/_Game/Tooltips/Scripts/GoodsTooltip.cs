using System.Collections.Generic;
using Dt.Attribute;
using Lean.Touch;
using UnityEngine;
using UnityEngine.EventSystems;

public class GoodsTooltip : MonoBehaviour
{
    [SerializeField, Required]
    private SnappingCamera snappingCamera;

    [SerializeField, Required]
    private RectTransform content;

    [SerializeField]
    private List<ProducibleGoods> producibleGoods;

    [SerializeField, ReadOnly]
    private List<GoodsRecipe> recipes;

    [SerializeField, ReadOnly]
    private Transform lastSibling;

    public void Initialize()
    {
        EventSystem.current.SetSelectedGameObject(gameObject, null);
        LeanTouch.OnFingerDown += OnFingerDownHandler;
        LeanTouch.OnFingerUp += OnFingerUpHandler;
        InitializeProducibleGoods();
    }

    private void InitializeProducibleGoods()
    {
        foreach (ProducibleGoods goods in this.producibleGoods)
        {
            goods.Initialize();
        }
    }

    private void OnFingerDownHandler(LeanFinger finger)
    {
        HideIfClickOutside(finger);
    }

    private void HideIfClickOutside(LeanFinger finger)
    {
        if (!gameObject.activeSelf) return;
        Vector3 fingerPos = finger.GetWorldPosition(CameraConstant.ZPosition);
        fingerPos = this.content.InverseTransformPoint(fingerPos);
        if (this.content.rect.Contains(fingerPos)) return;
        Hide();
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
        for (int i = 0; i < this.producibleGoods.Count; i++)
        {
            if (i < numberOfRecipes)
            {
                this.producibleGoods[i].Show(this.recipes[i]);
                this.producibleGoods[i].gameObject.SetActive(true);
            }
            else
            {
                this.producibleGoods[i].gameObject.SetActive(false);
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

    public void ResetContent()
    {
        this.content.gameObject.SetActive(true);
        if (this.lastSibling == null) return;
        this.lastSibling.SetParent(this.content);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}