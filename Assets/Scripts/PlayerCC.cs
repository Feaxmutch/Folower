using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(CharacterMovement))]
public class PlayerCC : MonoBehaviour
{
    private PlayerInputHandler _inputHandler;
    private CharacterMovement _movement;

    private void Awake()
    {
        _inputHandler = GetComponent<PlayerInputHandler>();
        _movement = GetComponent<CharacterMovement>();
    }

    private void Update()
    {
        _movement.Move(_inputHandler.InputDirection);

    }

    private void OnEnable()
    {
        _inputHandler.Jumped += _movement.Jump;
    }

    private void OnDisable()
    {
        _inputHandler.Jumped -= _movement.Jump;
    }
}
