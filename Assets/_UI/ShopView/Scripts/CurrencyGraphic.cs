using UnityEngine;

[CreateAssetMenu(fileName = "New Currency Graphic", menuName = "Currency Graphic")]
public class CurrencyGraphic : ScriptableObject
{
    public CurrencyType type;
    public Sprite icon;
}