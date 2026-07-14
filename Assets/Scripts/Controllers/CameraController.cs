using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] float smoothTime = 0.06f;
    [SerializeField] float dragSpeed = 1f;
    [SerializeField] Vector2 minPosition = Vector2.zero;
    [SerializeField] Vector2 maxPosition = Vector2.zero;
    [Header("Zoom")]
    [SerializeField] float zoomSpeed = 1f;
    [SerializeField] float minZoom = 2f;
    [SerializeField] float maxZoom = 8.4f;
    [SerializeField] float zoomSmoothTime = 0.06f;

    Vector3 targetPosition;
    Vector3 velocity = Vector3.zero;
    Vector2 lastDragWorld;
    [SerializeField] bool isDragging = false;
    float targetZoom;
    float zoomVelocity = 0f;

    void Start()
    {
        if (cam == null) cam = Camera.main;

        // 기본 맵 범위가 설정되지 않았다면 Util의 맵 크기를 사용
        if (minPosition == Vector2.zero && maxPosition == Vector2.zero)
        {
            float halfW = GameData.Instance.MapWidth * 0.5f;
            float halfH = GameData.Instance.MapHeight * 0.5f;
            minPosition = new Vector2(-halfW, -halfH);
            maxPosition = new Vector2(halfW, halfH);
        }

        targetPosition = transform.position;
        targetZoom = cam.orthographicSize;

        // InputManager 이벤트 구독
        if (Managers.Input != null)
        {
            Managers.Input.OnDragStarted -= OnDragStarted;
            Managers.Input.OnDragPerformed -= OnDragPerformed;
            Managers.Input.OnDragCanceled -= OnDragCanceled;
            Managers.Input.OnMouseScrollYPerformed -= OnScrolled;

            Managers.Input.OnDragStarted += OnDragStarted;
            Managers.Input.OnDragPerformed += OnDragPerformed;
            Managers.Input.OnDragCanceled += OnDragCanceled;
            Managers.Input.OnMouseScrollYPerformed += OnScrolled;
        }
    }

    void OnDestroy()
    {
        if (Managers.Input != null)
        {
            Managers.Input.OnDragStarted -= OnDragStarted;
            Managers.Input.OnDragPerformed -= OnDragPerformed;
            Managers.Input.OnDragCanceled -= OnDragCanceled;
            Managers.Input.OnMouseScrollYPerformed -= OnScrolled;
        }
    }

    void OnDragStarted(Vector2 world)
    {
        lastDragWorld = world;
        isDragging = true;
    }

    void OnDragPerformed(Vector2 world)
    {
        if (!isDragging) return;

        Vector2 diff = lastDragWorld - world;
        Vector3 delta = new Vector3(diff.x, diff.y, 0f) * dragSpeed;
        targetPosition += delta;
        lastDragWorld = world;

        ClampTarget();
    }

    void OnDragCanceled(Vector2 world)
    {
        isDragging = false;
    }

    void OnScrolled(float scrollValue)
    {
        if (cam == null || !cam.orthographic) return;

        // Simple zoom: adjust targetZoom by scroll delta and clamp
        targetZoom = Mathf.Clamp(targetZoom - scrollValue * zoomSpeed, minZoom, maxZoom);
        ClampTarget();
    }

    void LateUpdate()
    {
        if (cam != null)
        {
            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetZoom, ref zoomVelocity, zoomSmoothTime);
        }
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    void ClampTarget()
    {
        if (cam == null) return;

        float camHalfHeight = targetZoom;
        float camHalfWidth = camHalfHeight * cam.aspect;

        float minX = minPosition.x + camHalfWidth;
        float maxX = maxPosition.x - camHalfWidth;
        float minY = minPosition.y + camHalfHeight;
        float maxY = maxPosition.y - camHalfHeight;

        // 카메라가 보여주는 영역이 맵 크기보다 클 경우 중앙으로 고정
        if (minX > maxX)
        {
            float mid = (minPosition.x + maxPosition.x) * 0.5f;
            minX = maxX = mid;
        }
        if (minY > maxY)
        {
            float mid = (minPosition.y + maxPosition.y) * 0.5f;
            minY = maxY = mid;
        }

        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
    }
}
