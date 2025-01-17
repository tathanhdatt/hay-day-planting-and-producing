using System;
using UnityEngine;

public abstract class Dialog : MonoBehaviour
{
    public event Action<Dialog> OnHide;
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
        OnHide?.Invoke(this);
    }
}