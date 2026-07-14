using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class InputManager {
    PlayerInputActions _playerInput = null;

    public Action OnSnapPerformed;
    public Action<Vector2> OnClickPerformed;
    public Action<Vector2> OnDragStarted;
    public Action<Vector2> OnDragPerformed;
    public Action<Vector2> OnDragCanceled;
    public Action<float> OnMouseScrollYPerformed;
    public Vector2 MousePosition;
    bool _isDragging = false;

    public InputManager()
    {
        _playerInput = new @PlayerInputActions();

        _playerInput.Player.Snap.performed += _ => OnSnapPerformed?.Invoke();

        // Drag lifecycle: started, performed (click), canceled
        _playerInput.Player.Click.started += _ => {
            // If pointer is over UI, do not start gameplay drag.
            Vector2 screenPoint = _playerInput.Player.MousePoint.ReadValue<Vector2>();
            if (IsPointerOverUI(screenPoint)) {
                return;
            }

            _isDragging = true;
            Vector2 world = Camera.main.ScreenToWorldPoint(screenPoint);
            MousePosition = world;
            OnDragStarted?.Invoke(world);
        };

        _playerInput.Player.Click.performed += ctx => {
            // 기존 클릭 이벤트(단발 클릭) 호출 유지, 단 UI 위에서는 전달하지 않음
            Vector2 screenPoint = _playerInput.Player.MousePoint.ReadValue<Vector2>();
            if (IsPointerOverUI(screenPoint)) {
                return;
            }
            OnClicked(ctx);
        };

        bool IsPointerOverUI(Vector2 screenPoint) {
            if (EventSystem.current == null) return false;
            var eventData = new PointerEventData(EventSystem.current) { position = screenPoint };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            return results.Count > 0;
        }

        _playerInput.Player.Click.canceled += _ => {
            if (_isDragging) {
                Vector2 world = Camera.main.ScreenToWorldPoint(_playerInput.Player.MousePoint.ReadValue<Vector2>());
                MousePosition = world;
                OnDragCanceled?.Invoke(world);
                _isDragging = false;
            }
        };

        _playerInput.Player.MouseScrollY.performed += ctx => {
            float scrollValue = ctx.ReadValue<float>();
            OnMouseScrollYPerformed?.Invoke(scrollValue);
        };

        _playerInput.Enable();
    }

    public void Update()
    {
        // 드래그 중이면 매 프레임 현재 마우스 월드 좌표를 전달
        if (_isDragging) {
            Vector2 world = Camera.main.ScreenToWorldPoint(_playerInput.Player.MousePoint.ReadValue<Vector2>());
            MousePosition = world;
            OnDragPerformed?.Invoke(world);
        }

        // 임시 게임 종료 로직 (ESC 키)
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    void OnClicked(InputAction.CallbackContext context)
    {
        MousePosition = Camera.main.ScreenToWorldPoint(_playerInput.Player.MousePoint.ReadValue<Vector2>());
        OnClickPerformed?.Invoke(MousePosition);
    }
}
