using System;
using Cysharp.Threading.Tasks;
using Dt.Attribute;
using UnityEngine;
using UnityEngine.UI;

public class TabToggle : MonoBehaviour
{
    [SerializeField]
    private ShopItemType type;

    [SerializeField, Required]
    private Toggle toggle;

    [SerializeField, Required]
    private GameObject pressedGameObject;

    public ShopItemType Type => this.type;
    public event Action<TabToggle> OnTurnOn;

    public async UniTask Initialize()
    {
        this.toggle.onValueChanged.AddListener(OnValueChangedHandler);
        await UniTask.CompletedTask;
    }

    private void OnValueChangedHandler(bool isOn)
    {
        
        this.pressedGameObject.SetActive(isOn);
        if (isOn)
        {
            OnTurnOn?.Invoke(this);
        }
    }

    public void TurnOn()
    {
        this.toggle.isOn = true;
        this.toggle.enabled = false;
    }

    public void TurnOff()
    {
        this.toggle.isOn = false;
        this.toggle.enabled = true;
    }
}