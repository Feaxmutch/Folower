using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyMovement : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _gravityMultiplyer;
    [SerializeField] private Collider _mainCollider;

    private Rigidbody _rigidbody;
    private Vector2 _moveDirection;
    private bool _isJump;
    private List<Vector3> _groundNormals;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _groundNormals = new();
    }

    private void FixedUpdate()
    {
        ApplyForces();
        _groundNormals.Clear();
    }


    private void OnCollisionStay(Collision collision)
    {
        //var contacts = collision.contacts;
        //Vector3 footPosition = GetFootPosition();
        //var contactsForStairs = contacts.Where(contact => contact.point.y > footPosition.y + 0.1f && contact.point.y < footPosition.y + 1);

        //if (contactsForStairs.Count() > 0)
        //{
        //    Vector3 highestPosition = contactsForStairs.Select(contact => contact.point).OrderByDescending(point => point.y).First();
        //    transform.position = new Vector3(highestPosition.x, highestPosition.y + _mainCollider.bounds.extents.y, highestPosition.z);
        //}

        _groundNormals.AddRange(collision.contacts.Select(contact => contact.normal).Where(normal => normal.y >= 0.6f).ToList());
    }

    public void move(Vector2 direction)
    {
        _moveDirection = direction.normalized;
    }

    public void Jump()
    {
        _isJump = true;
    }

    private void ApplyForces()
    {
        Vector3 gravityVelocity = Vector3.up * Physics.gravity.y * _gravityMultiplyer;
        _rigidbody.AddForce(RotateByGround(gravityVelocity), ForceMode.Acceleration);

        if (IsOnGround())
        {
            Vector3 moveVelocity = GetMoveVelocity();
            _rigidbody.AddForce(RotateByGround(moveVelocity), ForceMode.VelocityChange);

            if (_isJump)
            {
                Vector3 jumpVelocity = Vector3.up * _jumpForce;
                _rigidbody.AddForce(RotateByGround(jumpVelocity), ForceMode.Impulse);
                _isJump = false;
            }
        }
    }

    private Vector3 GetMoveVelocity()
    {
        Vector3 forwardVelocity = transform.forward * _moveDirection.y;
        Vector3 rightVelocity = transform.right * _moveDirection.x;
        return (forwardVelocity + rightVelocity) * _speed;
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

    private Vector3 GetFootPosition()
    {
        float footY = transform.position.y - _mainCollider.bounds.extents.y;
        return new Vector3(transform.position.x, footY, transform.position.z);
    }
}
