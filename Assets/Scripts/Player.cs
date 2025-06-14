using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(RigidbodyMovement))]
public class Player : MonoBehaviour
{
    private PlayerInputHandler _inputHandler;
    private RigidbodyMovement _movement;

    private void Awake()
    {
        _inputHandler = GetComponent<PlayerInputHandler>();
        _movement = GetComponent<RigidbodyMovement>();
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
