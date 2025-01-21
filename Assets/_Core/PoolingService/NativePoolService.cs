using System;
using System.Collections.Generic;
using UnityEngine;

public class NativePoolService : MonoBehaviour, IPoolService
{
    private readonly Dictionary<Type, Stack<MonoBehaviour>> pools =
        new Dictionary<Type, Stack<MonoBehaviour>>();

    public T Spawn<T>(T prefab, Transform parent = null) where T : MonoBehaviour
    {
        if (prefab == null)
        {
            throw new ArgumentNullException(nameof(prefab));
        }

        bool hasPool = this.pools.ContainsKey(typeof(T));
        if (!hasPool)
        {
            WarmUp<T>();
        }

        bool hasInstance = this.pools[typeof(T)].Count > 0;
        T go;
        if (hasInstance)
        {
            go = this.pools[typeof(T)].Pop() as T;
            go?.transform.SetParent(parent);
        }
        else
        {
            go = Instantiate(prefab, parent);
        }

        return go;
    }


    public void Despawn<T>(T go) where T : MonoBehaviour
    {
        if (go == null)
        {
            throw new ArgumentNullException($"The go cannot be null.");
        }

        bool hasPool = this.pools.ContainsKey(typeof(T));
        if (!hasPool)
        {
            WarmUp<T>();
        }

        PushIntoPool(go);
        go.gameObject.SetActive(false);
    }

    private void WarmUp<T>() where T : MonoBehaviour
    {
        Stack<MonoBehaviour> newPool = new Stack<MonoBehaviour>();
        this.pools.Add(typeof(T), newPool);
    }

    private void PushIntoPool<T>(T prefab) where T : MonoBehaviour
    {
        this.pools[typeof(T)].Push(prefab);
        prefab.transform.SetParent(transform);
    }
}