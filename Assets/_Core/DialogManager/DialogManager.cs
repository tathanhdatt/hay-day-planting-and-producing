using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    private readonly Dictionary<Type, Stack<Dialog>> cachedDialogs =
        new Dictionary<Type, Stack<Dialog>>(10);

    [SerializeField]
    private List<Dialog> prefabs;

    public bool TryGetDialog<T>(out T dialog) where T : Dialog
    {
        AddKeyIfNotExist<T>();
        bool hasDialogTypeT = this.cachedDialogs[typeof(T)].Count > 0;
        if (!hasDialogTypeT) return TryInstantiateDialog(out dialog);
        dialog = this.cachedDialogs[typeof(T)].Pop() as T;
        return true;
    }

    private void AddKeyIfNotExist<T>() where T : Dialog
    {
        bool hasKeyTypeT = this.cachedDialogs.ContainsKey(typeof(T));
        if (!hasKeyTypeT) AddKey<T>();
    }

    private void AddKey<T>() where T : Dialog
    {
        this.cachedDialogs.Add(typeof(T), new Stack<Dialog>());
    }

    private bool TryInstantiateDialog<T>(out T dialog) where T : Dialog
    {
        dialog = null;
        T prefab = GetPrefab<T>();
        if (prefab == null) return false;
        dialog = Instantiate(prefab, transform);
        dialog.gameObject.SetActive(false);
        this.cachedDialogs[typeof(T)].Push(dialog);
        return true;
    }

    public void AddDialog<T>(T dialog) where T : Dialog
    {
        AddKeyIfNotExist<T>();
        this.cachedDialogs[typeof(T)].Push(dialog);
    }

    private T GetPrefab<T>() where T : Dialog
    {
        foreach (Dialog dialog in this.prefabs)
        {
            if (dialog is T prefab)
            {
                return prefab;
            }
        }

        throw new ArgumentException($"There is no prefab dialog type of {typeof(T)}");
    }
}