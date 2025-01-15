using Dt.Attribute;
using UnityEngine;

public class SnappingCamera : MonoBehaviour
{
    [SerializeField, Required]
    private Camera cam;


    [SerializeField, Required]
    private RectTransform bound;

    [Title("Offset")]
    [SerializeField]
    private float left;

    [SerializeField]
    private float top;

    [SerializeField]
    private float right;

    [SerializeField]
    private float down;

    [SerializeField, ReadOnly]
    private Transform source;

    [SerializeField, ReadOnly]
    private Vector3 center;

    private RectTransform RectTransform => transform as RectTransform;

    public void SetSource(Transform source)
    {
        this.source = source;
    }

    private void OnEnable()
    {
        CalculateAnchoredPosition();
        SnapCamera();
    }

    private void CalculateAnchoredPosition()
    {
        this.center = this.bound.InverseTransformPoint(this.source.position);
        ClampPosition();
    }

    [Button("Test Clamp Position")]
    private void ClampCurrentPos()
    {
        this.center = RectTransform.localPosition;
        ClampPosition();
    }

    private void ClampPosition()
    {
        this.center.y =
            Mathf.Clamp(this.center.y, GetLowerBound(), GetUpperBound());
        this.center.x =
            Mathf.Clamp(this.center.x, GetLeftBound(), GetRightBound());
        RectTransform.localPosition = this.center;
    }

    private float GetLeftBound()
    {
        float leftBound = -this.bound.rect.width / 2 + this.left;
        return leftBound;
    }

    private float GetRightBound()
    {
        float rightBound = this.bound.rect.width / 2 + this.right;
        return rightBound;
    }

    private float GetUpperBound()
    {
        float upperBounds = this.bound.rect.height / 2 + this.top;
        return upperBounds;
    }

    private float GetLowerBound()
    {
        float lowerBound = -this.bound.rect.height / 2 + this.down;
        return lowerBound;
    }

    private void SnapCamera()
    {
        Vector3 origin = transform.position;
        Vector3 offset = this.cam.transform.position - origin;
        Vector3 dest = Vector3.zero;
        dest.x = this.source.position.x + offset.x;
        dest.y = this.source.position.y + offset.y;
        Messenger.Broadcast(Message.MoveCameraTo, dest);
    }

    private void Update()
    {
        if (this.source != null)
        {
            RectTransform.localPosition = this.center;
        }
    }
}