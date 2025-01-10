using System.Collections.Generic;
using Dt.Attribute;
using Lean.Touch;
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

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        LeanTouch.OnGesture += OnGestureHandler;
    }

    private void OnGestureHandler(List<LeanFinger> fingers)
    {
        if (GameState.isEditing) return;
        if (fingers.Count != NumberOfZoomFingers) return;
        if (!IsAnyFingerMoved(fingers)) return;
        AdjustOrthographicSize(fingers[0], fingers[1]);
    }

    private bool IsAnyFingerMoved(List<LeanFinger> fingers)
    {
        foreach (LeanFinger finger in fingers)
        {
            if (IsFingerMoved(finger))
            {
                return true;
            }
        }
        
        return false;
    }

    private bool IsFingerMoved(LeanFinger finger)
    {
        return finger.ScreenPosition != finger.LastScreenPosition;
    }

    private void AdjustOrthographicSize(LeanFinger first, LeanFinger second)
    {
        Vector2 firstFingerPos = first.ScreenPosition;
        Vector2 secondFingerPos = second.ScreenPosition;
        this.cam.orthographicSize = CalcOrthographicSize(firstFingerPos, secondFingerPos);
    }

    private float CalcOrthographicSize(Vector2 firstFingerPos, Vector2 secondFingerPos)
    {
        float sqrtMagnitude = (firstFingerPos - secondFingerPos).sqrMagnitude;
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

    private void OnDestroy()
    {
        LeanTouch.OnGesture -= OnGestureHandler;
    }
}