using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem.HID;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyMovement : Movement
{
    private Rigidbody _rigidbody;
    private List<Vector3> _groundNormals;

    protected override void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _groundNormals = new();
    }

    protected override void FixedUpdate()
    {
        ApplyForces();
        _groundNormals.Clear();
        LastVelocity = _rigidbody.velocity;
    }

    private void OnCollisionStay(Collision collision)
    {
        _groundNormals.AddRange(collision.contacts.Where(contact => contact.normal.y >= GroundNormalYLimit).Select(contact => contact.normal).ToList());
    }

    private void ApplyForces()
    {
        Vector3 gravityVelocity = Physics.gravity * GravityMultiplyer;
        Vector3 moveVelocity = GetMoveVelocity();
        _rigidbody.AddForce(RotateByGround(gravityVelocity), ForceMode.Acceleration);
        _rigidbody.AddForce(RotateByGround(moveVelocity), ForceMode.VelocityChange);
    }

    private Vector3 RotateByGround(Vector3 vector)
    {
        return Quaternion.FromToRotation(Vector3.up, GetAverageGroundNormal()) * vector;
    }

    private Vector3 GetAverageGroundNormal()
    {
        if (IsOnGround() == false)
        {
            return Vector3.up;
        }

        Vector3 averageNormal = Vector3.zero;

        foreach (var normal in _groundNormals)
        {
            averageNormal += normal;
        }

        return averageNormal.normalized;
    }

    private bool IsOnGround()
    {
        return _groundNormals.Count > 0;
    }
}
