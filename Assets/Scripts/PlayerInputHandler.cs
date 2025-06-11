using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput _playerInput;

    public event Action Jumped;

    public Vector2 InputDirection { get; private set; }

    private void Awake()
    {
        _playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        _playerInput.Player.Move.performed += OnMove;
        _playerInput.Player.Move.canceled += OnMove;
        _playerInput.Player.Jump.started += OnJump;
        _playerInput.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Player.Move.performed -= OnMove;
        _playerInput.Player.Move.canceled -= OnMove;
        _playerInput.Player.Jump.started -= OnJump;
        _playerInput.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        InputDirection = context.action.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        Jumped?.Invoke();
    }
}
