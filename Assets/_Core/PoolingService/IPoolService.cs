using Core.Service;
using UnityEngine;

public interface IPoolService : IService
{
    T Spawn<T>(T prefab, Transform parent = null) where T : MonoBehaviour;
    void Despawn<T>(T go) where T : MonoBehaviour;
}