using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : Movement
{
    private CharacterController _controller;

    protected override void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    protected override void Update()
    {
        Vector3 velocity = Vector3.zero;
        velocity += Physics.gravity * GravityMultiplyer * Time.deltaTime;
        velocity += GetMoveVelocity() * Time.deltaTime;
        LastVelocity = _controller.velocity;
        _controller.Move(velocity);
    }
}
