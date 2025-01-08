using Dt.Attribute;
using UnityEngine;

public class GameStateDebugger : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private bool isEditing;

    private void Update()
    {
        this.isEditing = GameState.isEditing;
    }
}