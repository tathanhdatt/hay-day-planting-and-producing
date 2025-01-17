using Dt.Attribute;
using UnityEngine;

public class HarvestTooltip : MonoBehaviour
{
    [SerializeField, Required]
    private SnappingCamera snappingCamera;

    [SerializeField, Required]
    private ProducedSlotCleaner cleaner;
    
    [SerializeField, Required]
    private ClickOutsideHider clickOutsideHider;

    public void Initialize()
    {
        this.cleaner.Initialize();
        this.clickOutsideHider.Initialize();
    }

    public void Show(Transform source)
    {
        this.snappingCamera.SetSource(source);
        gameObject.SetActive(true);
    }
}