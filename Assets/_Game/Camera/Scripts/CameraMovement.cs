using Dt.Attribute;
using UnityEngine;
using UnityEngine.EventSystems;

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

    [Title("")]
    [SerializeField, Required]
    private Camera cam;

    [SerializeField]
    private float speedInEditing;

    [SerializeField, ReadOnly]
    private Vector3 lastTouchPos;

    [SerializeField, ReadOnly]
    private bool canMove;

    private Touch touch;

    private void Update()
    {
        if (GameState.isEditing)
        {
            this.canMove = false;
            MoveCameraWhileEditing();
        }
        else
        {
            Move();
        }
    }

    private void MoveCameraWhileEditing()
    {
        if (!HasTouch()) return;
        this.touch = Input.GetTouch(0);
        if (!IsClosedDeviceBoundary()) return;
        Vector3 touchWorldPos = this.cam.ScreenToWorldPoint(this.touch.position);
        Vector3 pos = Vector3.Lerp(
            this.cam.transform.position,
            touchWorldPos,
            Time.deltaTime * this.speedInEditing);
        this.cam.transform.position = ClampCameraPosition(pos);
    }

    private bool IsClosedDeviceBoundary()
    {
        Vector3 viewportPoint = this.cam.ScreenToViewportPoint(this.touch.position);
        bool isInHorizontalBoundary = viewportPoint.x is < leftBoundary or > rightBoundary;
        bool isInVerticalBoundary = viewportPoint.y is < bottomBoundary or > topBoundary;
        return isInHorizontalBoundary || isInVerticalBoundary;
    }

    private void Move()
    {
        bool isZooming = Input.touchCount == CameraZoom.NumberOfZoomFingers;
        if (!HasTouch() || isZooming) return;
        this.touch = Input.GetTouch(0);
        HandleByTouchPhase();
    }

    private bool HasTouch()
    {
        return Input.touchCount > 0;
    }

    private void HandleByTouchPhase()
    {
        switch (this.touch.phase)
        {
            case TouchPhase.Began:
                OnTouchBegan();
                break;
            case TouchPhase.Moved:
                OnTouchMoved();
                break;
            case TouchPhase.Ended:
                OnTouchEnded();
                break;
        }
    }

    private void OnTouchBegan()
    {
        bool isOverUI = EventSystem.current.IsPointerOverGameObject();
        if (isOverUI) return;
        this.canMove = true;
        UpdateLastTouchPos();
    }

    private void UpdateLastTouchPos()
    {
        this.lastTouchPos = this.cam.ScreenToWorldPoint(this.touch.position);
    }

    private void OnTouchMoved()
    {
        if (!this.canMove) return;
        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        Vector3 direction = this.lastTouchPos - this.cam.ScreenToWorldPoint(this.touch.position);
        Vector3 position = this.cam.transform.position + direction;
        this.cam.transform.position = ClampCameraPosition(position);
    }

    private Vector3 ClampCameraPosition(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, this.xMin, this.xMax);
        position.y = Mathf.Clamp(position.y, this.yMin, this.yMax);
        position.z = -10;
        return position;
    }

    private void OnTouchEnded()
    {
        this.canMove = false;
    }
}