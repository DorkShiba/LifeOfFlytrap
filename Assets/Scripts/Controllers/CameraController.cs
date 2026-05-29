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
    [SerializeField] float maxZoom = 12f;
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
        if (minPosition == Vector2.zero && maxPosition == Vector2.zero) {
            float halfW = Util.MapWidth * 0.5f;
            float halfH = Util.MapHeight * 0.5f;
            minPosition = new Vector2(-halfW, -halfH);
            maxPosition = new Vector2(halfW, halfH);
        }

        targetPosition = transform.position;
        targetZoom = cam.orthographicSize;

        // InputManager 이벤트 구독
        if (Managers.Input != null) {
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

    void OnDestroy() {
        if (Managers.Input != null) {
            Managers.Input.OnDragStarted -= OnDragStarted;
            Managers.Input.OnDragPerformed -= OnDragPerformed;
            Managers.Input.OnDragCanceled -= OnDragCanceled;
            Managers.Input.OnMouseScrollYPerformed -= OnScrolled;
        }
    }

    void OnDragStarted(Vector2 world) {
        lastDragWorld = world;
        isDragging = true;
    }

    void OnDragPerformed(Vector2 world) {
        if (!isDragging) return;

        Vector2 diff = lastDragWorld - world;
        Vector3 delta = new Vector3(diff.x, diff.y, 0f) * dragSpeed;
        targetPosition += delta;
        lastDragWorld = world;

        ClampTarget();
    }

    void OnDragCanceled(Vector2 world) {
        isDragging = false;
    }

    void OnScrolled(float scrollValue) {
        if (cam == null || !cam.orthographic) return;

        // Simple zoom: adjust targetZoom by scroll delta and clamp
        targetZoom = Mathf.Clamp(targetZoom - scrollValue * zoomSpeed, minZoom, maxZoom);
    }

    void LateUpdate()
    {
        if (cam != null) {
            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetZoom, ref zoomVelocity, zoomSmoothTime);
        }
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    void ClampTarget() {
        targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition.x, maxPosition.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minPosition.y, maxPosition.y);
    }
}
