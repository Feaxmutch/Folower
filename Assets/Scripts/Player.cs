using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(Movement))]
public class Player : MonoBehaviour
{
    private PlayerInputHandler _inputHandler;
    private Movement _movement;

    private void Awake()
    {
        _inputHandler = GetComponent<PlayerInputHandler>();
        _movement = GetComponent<Movement>();
    }

    private void Update()
    {
        _movement.Move(_inputHandler.InputDirection);

    }
}
