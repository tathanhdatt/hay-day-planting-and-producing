using Dt.Attribute;
using Lean.Touch;
using UnityEngine;
using UnityEngine.UI;

public class HarvestTooltip : MonoBehaviour
{
    [SerializeField, Required]
    private SnappingCamera snappingCamera;

    [SerializeField, Required]
    private ProducedSlotCleaner cleaner;

    [SerializeField, Required]
    private ClickOutsideHider clickOutsideHider;

    [SerializeField, Required]
    private Image background;

    public void Initialize()
    {
        this.cleaner.Initialize();
        this.clickOutsideHider.Initialize();
        this.cleaner.OnCleaned += OnCleanedHandler;
        LeanTouch.OnFingerUp += OnFingerUpHandler;
    }


    private void OnCleanedHandler()
    {
        this.background.enabled = false;
    }

    private void OnFingerUpHandler(LeanFinger finger)
    {
        if (this.background.enabled) return;
        gameObject.SetActive(false);
    }

    public void Show(Transform source)
    {
        this.background.enabled = true;
        this.snappingCamera.SetSource(source);
        gameObject.SetActive(true);
    }
}