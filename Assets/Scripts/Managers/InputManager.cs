using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputManager {
    PlayerInputActions _playerInput = null;

    public Action OnSnapPerformed;
    public Action<Vector2> OnClickPerformed;
    public Vector2 MousePosition;

    public InputManager()
    {
        _playerInput = new @PlayerInputActions();

        _playerInput.Player.Snap.performed += _ => OnSnapPerformed?.Invoke();
        _playerInput.Player.Click.performed += ctx => OnClicked(ctx);

        _playerInput.Enable();
    }

    public void OnUpdate()
    {
        
    }

    void OnClicked(InputAction.CallbackContext context)
    {
        MousePosition = Camera.main.ScreenToWorldPoint(_playerInput.Player.MousePoint.ReadValue<Vector2>());
        OnClickPerformed?.Invoke(MousePosition);
    }
}
