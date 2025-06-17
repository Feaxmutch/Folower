using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _gravityMultiplyer;
    [SerializeField] private float _jumpForce;

    private CharacterController _controller;
    private Vector2 _moveDirection;
    private bool _isJump;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector3 gravityVelocity = Physics.gravity * _gravityMultiplyer * Time.deltaTime;
        _controller.Move(gravityVelocity);

        if (_controller.isGrounded)
        {
            Vector3 moveVelocity = GetMoveVelocity() * Time.deltaTime;
            _controller.Move(moveVelocity);

            if (_isJump)
            {
                Vector3 jumpVelocity = Vector3.up * _jumpForce;
                _controller.Move(jumpVelocity);
                _isJump = false;
            }
        }
    }

    public void Jump()
    {
        _isJump = true;
    }

    public void Move(Vector2 direction)
    {
        _moveDirection = direction.normalized;
    }

    public void Stop()
    {
        _moveDirection = Vector2.zero;
    }

    private Vector3 GetMoveVelocity()
    {
        Vector3 forwardVelocity = transform.forward * _moveDirection.y;
        Vector3 rightVelocity = transform.right * _moveDirection.x;
        return (forwardVelocity + rightVelocity) * _speed;
    }

    
}
