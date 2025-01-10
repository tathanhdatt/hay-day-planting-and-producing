using Dt.Attribute;
using Lean.Touch;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private const float topBoundary = 0.8f;
    private const float bottomBoundary = 0.2f;
    private const float leftBoundary = 0.3f;
    private const float rightBoundary = 0.7f;

    [Title("Bound")]
    [SerializeField]
    private float xMin;

    [SerializeField]
    private float xMax;

    [SerializeField]
    private float yMin;

    [SerializeField]
    private float yMax;

    [Title]
    [SerializeField, Required]
    private Camera cam;

    [SerializeField]
    private float speedInEditing;

    [SerializeField, ReadOnly]
    private Vector3 lastTouchPos;

    [SerializeField, ReadOnly]
    private bool canMove;

    private Touch touch;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        LeanTouch.OnFingerDown += OnFingerDownHandler;
        LeanTouch.OnFingerUpdate += OnFingerUpdateHandler;
        LeanTouch.OnFingerUp += OnFingerUpHandler;
    }


    private void OnFingerDownHandler(LeanFinger finger)
    {
        if (finger.IsOverGui) return;
        this.canMove = true;
    }

    private void OnFingerUpdateHandler(LeanFinger finger)
    {
        if (GameState.isEditing)
        {
            MoveCameraWhileEditing(finger);
        }
        else
        {
            Move(finger);
        }
    }

    private void OnFingerUpHandler(LeanFinger finger)
    {
        this.canMove = false;
    }

    private void MoveCameraWhileEditing(LeanFinger finger)
    {
        if (!IsClosedDeviceBoundary(finger)) return;
        Vector3 worldPos = finger.GetWorldPosition(CameraConstant.ZPosition);
        Vector3 pos = Vector3.Lerp(
            this.cam.transform.position,
            worldPos,
            Time.deltaTime * this.speedInEditing);
        this.cam.transform.position = ClampCameraPosition(pos);
    }

    private void Move(LeanFinger finger)
    {
        bool isZooming = LeanTouch.Fingers.Count == 2;
        if (isZooming) return;
        if (!this.canMove) return;
        Vector3 delta = finger.GetWorldDelta(CameraConstant.ZPosition);
        Vector3 newPos = this.cam.transform.position - delta;
        this.cam.transform.position = ClampCameraPosition(newPos);
    }

    private bool IsClosedDeviceBoundary(LeanFinger finger)
    {
        Vector3 viewportPoint = this.cam.ScreenToViewportPoint(finger.ScreenPosition);
        bool isInHorizontalBoundary = viewportPoint.x is < leftBoundary or > rightBoundary;
        bool isInVerticalBoundary = viewportPoint.y is < bottomBoundary or > topBoundary;
        return isInHorizontalBoundary || isInVerticalBoundary;
    }


    private Vector3 ClampCameraPosition(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, this.xMin, this.xMax);
        position.y = Mathf.Clamp(position.y, this.yMin, this.yMax);
        position.z = CameraConstant.ZPosition;
        return position;
    }

    private void OnDestroy()
    {
        LeanTouch.OnFingerDown -= OnFingerDownHandler;
        LeanTouch.OnFingerUpdate -= OnFingerUpdateHandler;
        LeanTouch.OnFingerUp -= OnFingerUpHandler;
    }
}