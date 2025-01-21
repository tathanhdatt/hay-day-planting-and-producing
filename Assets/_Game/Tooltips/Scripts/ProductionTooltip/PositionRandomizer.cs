using Dt.Attribute;
using Dt.Extension;
using UnityEngine;

public class PositionRandomizer : MonoBehaviour
{
    [Title("X")]
    [SerializeField, Range(0f, 10f)]
    private float minX;

    [SerializeField, Range(0f, 10f)]
    private float maxX;

    [Title("Y")]
    [SerializeField, Range(0f, 10f)]
    private float minY;

    [SerializeField, Range(0, 10f)]
    private float maxY;

    public void Randomize()
    {
        float x = Random.Range(this.minX, this.maxX);
        float y = Random.Range(this.minY, this.maxY);
        transform.position = transform.position.Add(x, y);
    }
}