using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RigidbodyMovement))]
public class Folower : MonoBehaviour
{
    [SerializeField] private float _minDistance;
    [SerializeField] private Player _target;

    private RigidbodyMovement _movement;

    private void Awake()
    {
        _movement = GetComponent<RigidbodyMovement>();
    }

    private void Update()
    {
        if (Vector3.Distance(_target.transform.position, transform.position) < _minDistance)
        {
            _movement.Move(Vector2.zero);
            return;
        }

        Vector3 moveDirection = transform.rotation * (transform.position - _target.transform.position);
        Debug.Log(moveDirection);
        _movement.Move(new Vector2(moveDirection.x, moveDirection.z));
    }
}
