using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputManager {
    PlayerInputActions _playerInput = null;

    public Action OnSnapPerformed;

    public InputManager()
    {
        _playerInput = new @PlayerInputActions();

        _playerInput.Player.Snap.performed += _ => OnSnapPerformed?.Invoke();

        _playerInput.Enable();
    }

    public void OnUpdate()
    {
        
    }
}
