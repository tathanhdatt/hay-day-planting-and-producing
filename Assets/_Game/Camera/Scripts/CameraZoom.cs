using Dt.Attribute;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public const int NumberOfZoomFingers = 2;
    [SerializeField, Required]
    private Camera cam;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float minZoom;

    [SerializeField]
    private float maxZoom;

    [SerializeField]
    private float alignSpeed;

    [SerializeField, ReadOnly]
    private Vector3 offset;

    [SerializeField, ReadOnly]
    private Vector3 pointBetweenTwoTouch;

    [SerializeField, ReadOnly]
    private float lastSqrtMagnitude;

    [SerializeField, Required]
    private Transform pivot;

    private void Update()
    {
        Zoom();
    }

    private void Zoom()
    {
        if (GameState.isEditing) return;
        bool hasTwoFinger = Input.touchCount == NumberOfZoomFingers;
        if (!hasTwoFinger) return;
        AdjustCameraOrthographicSize();
        // AlignCamera();
    }

    private void AdjustCameraOrthographicSize()
    {
        Touch firstTouch = Input.GetTouch(0);
        Touch secondTouch = Input.GetTouch(1);
        bool isFirstTouchMoved = firstTouch.phase == TouchPhase.Moved;
        bool isSecondTouchMoved = secondTouch.phase == TouchPhase.Moved;
        if (!isFirstTouchMoved && !isSecondTouchMoved) return;
        this.cam.orthographicSize = CalcOrthographicSize(firstTouch, secondTouch);
    }

    private float CalcOrthographicSize(Touch firstTouch, Touch secondTouch)
    {
        float sqrtMagnitude = (firstTouch.position - secondTouch.position).sqrMagnitude;
        float sign = Mathf.Sign(this.lastSqrtMagnitude - sqrtMagnitude);
        float size = this.cam.orthographicSize + sign * this.speed * Time.deltaTime;
        size = Mathf.Clamp(size, this.minZoom, this.maxZoom);
        this.lastSqrtMagnitude = sqrtMagnitude;
        return size;
    }

    private void AlignCamera()
    {
        Touch firstTouch = Input.GetTouch(0);
        Touch secondTouch = Input.GetTouch(1);
        CalcOffset(firstTouch, secondTouch);
        Vector3 pos = Vector3.Lerp(
            this.cam.transform.position + this.offset,
            this.pointBetweenTwoTouch,
            this.alignSpeed * Time.deltaTime);
        pos -= this.offset;
        pos.z = this.cam.transform.position.z;
        this.cam.transform.position = pos;
    }

    private void CalcOffset(Touch firstTouch, Touch secondTouch)
    {
        Vector3 firstWorldPos = this.cam.ScreenToWorldPoint(firstTouch.position);
        Vector3 secondWorldPos = this.cam.ScreenToWorldPoint(secondTouch.position);
        this.pointBetweenTwoTouch = Vector3.Lerp(firstWorldPos, secondWorldPos, 0.5f);
        this.offset = this.pointBetweenTwoTouch - this.cam.transform.position;
        this.pivot.position = this.pointBetweenTwoTouch;
    }
}