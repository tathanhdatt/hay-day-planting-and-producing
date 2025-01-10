using System.Collections.Generic;
using Dt.Attribute;
using Lean.Touch;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private const int numberOfZoomFingers = 2;

    [SerializeField, Required]
    private Camera cam;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float minZoom;

    [SerializeField]
    private float maxZoom;

    [SerializeField, ReadOnly]
    private float lastSqrtMagnitude;

    [SerializeField, ReadOnly]
    private Vector3 beginZoomWorldPos;

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
        if (fingers.Count != numberOfZoomFingers) return;
        if (IsAnyFingerOverGUI(fingers)) return;
        if (fingers[0].Down || fingers[1].Down)
        {
            OnBeginZoom(fingers[0], fingers[1]);
        }

        if (!IsAnyFingerMoved(fingers)) return;
        AdjustOrthographicSize(fingers[0], fingers[1]);
        AlignCamera(fingers[0].ScreenPosition, fingers[1].ScreenPosition);
        
    }

    private void OnBeginZoom(LeanFinger first, LeanFinger second)
    {
        Vector2 beginZoomScreenPos = Vector2.Lerp(first.ScreenPosition, second.ScreenPosition, 0.5f);
        this.beginZoomWorldPos = this.cam.ScreenToWorldPoint(beginZoomScreenPos);
    }

    private bool IsAnyFingerOverGUI(List<LeanFinger> fingers)
    {
        foreach (LeanFinger finger in fingers)
        {
            if (finger.IsOverGui)
            {
                return true;
            }
        }

        return false;
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
        if (Mathf.Approximately(sqrtMagnitude, this.lastSqrtMagnitude))
        {
            return this.cam.orthographicSize;
        }
        float sign = Mathf.Sign(this.lastSqrtMagnitude - sqrtMagnitude);
        float size = this.cam.orthographicSize + sign * this.speed * Time.deltaTime;
        size = Mathf.Clamp(size, this.minZoom, this.maxZoom);
        this.lastSqrtMagnitude = sqrtMagnitude;
        return size;
    }

    private void AlignCamera(Vector2 first, Vector2 second)
    {
        Vector2 centerScreenPoint = Vector2.Lerp(first, second, 0.5f);
        Vector3 centerWorldPos = this.cam.ScreenToWorldPoint(centerScreenPoint);
        Vector3 worldOffset = centerWorldPos - this.cam.transform.position;
        this.cam.transform.position = this.beginZoomWorldPos - worldOffset;
    }

    private void OnDestroy()
    {
        LeanTouch.OnGesture -= OnGestureHandler;
    }
}