using UnityEngine;

public class NullPoolService : IPoolService
{
    private static NullPoolService instance;

    public static NullPoolService Instance => instance ??= new NullPoolService();

    public T Spawn<T>(T prefab, Transform parent = null) where T : MonoBehaviour
    {
        return null;
    }

    public void Despawn<T>(T go) where T : MonoBehaviour
    {
        throw new System.NotImplementedException();
    }
}