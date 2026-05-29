using UnityEngine;
using UnityEngine.InputSystem;
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
            _isDragging = true;
            Vector2 world = Camera.main.ScreenToWorldPoint(_playerInput.Player.MousePoint.ReadValue<Vector2>());
            MousePosition = world;
            OnDragStarted?.Invoke(world);
        };

        _playerInput.Player.Click.performed += ctx => {
            // 기존 클릭 이벤트(단발 클릭) 호출 유지
            OnClicked(ctx);
        };

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
    }

    void OnClicked(InputAction.CallbackContext context)
    {
        MousePosition = Camera.main.ScreenToWorldPoint(_playerInput.Player.MousePoint.ReadValue<Vector2>());
        OnClickPerformed?.Invoke(MousePosition);
    }
}
