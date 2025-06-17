using UnityEngine;

public abstract class Movement : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _gravityMultiplyer;
    [SerializeField] private Collider _mainCollider;
    [SerializeField] protected float _groundNormalYLimit = 0.7f;
    [SerializeField] private float _minStepHeight = 0.3f;
    [SerializeField] private float _maxStepHeight = 0.9f;
    [SerializeField] private float _stairStepOffset = 0.1f;

    protected Vector3 LastVelocity { get; set; }

    protected Vector2 MoveDirection { get; private set; }

    protected float Speed => _speed;
    protected float GravityMultiplyer => _gravityMultiplyer;

    protected virtual void Awake()
    {
        
    }

    protected virtual void Update()
    {
        
    }

    protected virtual void FixedUpdate()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        TryStepToStair();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        TryStepToStair();
    }

    private void OnValidate()
    {
        if (_minStepHeight < (float)default)
        {
            _minStepHeight = default;
        }

        if (_minStepHeight > _maxStepHeight)
        {
            _maxStepHeight = _minStepHeight;
        }
    }

    public void Move(Vector2 direction)
    {
        MoveDirection = direction.normalized;
    }

    public void Stop()
    {
        MoveDirection = Vector2.zero;
    }

    protected Vector3 GetMoveVelocity()
    {
        Vector3 forwardVelocity = transform.forward * MoveDirection.y;
        Vector3 rightVelocity = transform.right * MoveDirection.x;
        return (forwardVelocity + rightVelocity) * _speed;
    }

    private bool CanStepToStair(out Vector3 stepPosition)
    {
        Vector3 footPosition = GetFootPosition();
        float colliderHeight = GetAverageColliderHeight();
        float halfHeight = colliderHeight / 2;
        Vector3 forwardRayOrigin = footPosition + (Vector3.up * _maxStepHeight);
        Vector3 horisontalVelocity = new(LastVelocity.x, 0, LastVelocity.z);
        Ray forwardRay = new(forwardRayOrigin, horisontalVelocity.normalized);

        if (Physics.Raycast(forwardRay, out RaycastHit forwardHit, colliderHeight + _stairStepOffset))
        {
            stepPosition = Vector3.zero;
            return false;
        }

        Vector3 downwardsRayOrigin = forwardRayOrigin + (horisontalVelocity.normalized * (halfHeight + _stairStepOffset));
        Ray downwardsRay = new(downwardsRayOrigin, Vector3.down);

        if (Physics.Raycast(downwardsRay, out RaycastHit stepHit, _maxStepHeight - _minStepHeight) == false)
        {
            stepPosition = Vector3.zero;
            return false;
        }

        stepPosition = stepHit.point;
        return true;
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

    private void TryStepToStair()
    {
        if (CanStepToStair(out Vector3 stepPosition))
        {
            transform.position = stepPosition + (Vector3.up * _mainCollider.bounds.extents.y);
        }
    }
}
