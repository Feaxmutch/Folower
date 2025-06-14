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
    private float _groundNormalYLimit = 0.7f;
    private float _minStepHeight = 0.3f;
    private float _maxStepHeight = 0.9f;
    private Vector3 _lastVelocity;
    private float _stairStepOffset = 0.1f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _groundNormals = new();
    }

    private void FixedUpdate()
    {
        ApplyForces();
        _groundNormals.Clear();
        _lastVelocity = _rigidbody.velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {

        var potencialStairContacts = collision.contacts.Where(contact => contact.normal.y < _groundNormalYLimit);

        if (potencialStairContacts.Count() == 0)
        {
            return;
        }

        ContactPoint stairPoint = potencialStairContacts.OrderByDescending(contact => contact.point.y).First();
        Vector3 footPosition = GetFootPosition();

        if (stairPoint.point.y < footPosition.y + _maxStepHeight == false)
        {
            return;
        }

        Debug.Log(_lastVelocity);
        Vector3 forwardRayOrigin = new(stairPoint.point.x, footPosition.y + _maxStepHeight, stairPoint.point.z);
        Vector3 horisontalVelocity = new(_lastVelocity.x, 0, _lastVelocity.z);
        Ray forwardRay = new(forwardRayOrigin, horisontalVelocity.normalized);
        float colliderHeight = GetAverageColliderHeight();
        float halfHeight = colliderHeight / 2;

        if (Physics.Raycast(forwardRay, out RaycastHit testHit, halfHeight + _stairStepOffset))
        {
            Debug.DrawRay(forwardRayOrigin, horisontalVelocity.normalized * (halfHeight + _stairStepOffset), Color.red, 30);
            return;
        }

        Debug.DrawRay(forwardRayOrigin, horisontalVelocity.normalized * (halfHeight + _stairStepOffset), Color.green, 30);

        Vector3 downwardsRayOrigin = forwardRayOrigin + (horisontalVelocity.normalized * _stairStepOffset);
        Ray downwardsRay = new(downwardsRayOrigin, Vector3.down);

        if (Physics.Raycast(downwardsRay, out RaycastHit hit,_maxStepHeight - _minStepHeight) == false)
        {
            Debug.DrawRay(downwardsRayOrigin, Vector3.down * (_maxStepHeight - _minStepHeight), Color.red, 30);
            return;
        }

        Debug.DrawRay(downwardsRayOrigin, Vector3.down * (_maxStepHeight - _minStepHeight), Color.green, 30);

        transform.position = hit.point + (Vector3.up * _mainCollider.bounds.extents.y);
    }

    private void OnCollisionStay(Collision collision)
    {
        _groundNormals.AddRange(collision.contacts.Where(contact => contact.normal.y >= _groundNormalYLimit).Select(contact => contact.normal).ToList());
    }

    public void Move(Vector2 direction)
    {
        _moveDirection = direction.normalized;
    }

    public void Stop()
    {
        _moveDirection = Vector2.zero;
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

    private float GetAverageColliderHeight()
    {
        int axesCount = 2;
        Vector3 size = _mainCollider.bounds.size;
        return (size.x + size.z) / axesCount;
    }
}
