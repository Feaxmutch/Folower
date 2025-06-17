using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RigidbodyMovement))]
public class Folower : MonoBehaviour
{
    [SerializeField] private float _maxDistance;
    [SerializeField] private float _minDistance;
    [SerializeField] private Player _target;

    private RigidbodyMovement _movement;

    private void Awake()
    {
        _movement = GetComponent<RigidbodyMovement>();
    }

    private void Update()
    {
        Vector3 moveDirection = Vector3.zero;
        float distance = Vector3.Distance(_target.transform.position, transform.position);

        if (_minDistance <= distance && distance <= _maxDistance)
        {
            moveDirection = Vector3.zero;
        }

        if (distance > _maxDistance)
        {
            moveDirection = transform.rotation * (transform.position - _target.transform.position);
        }

        if (distance < _minDistance)
        {
            moveDirection = transform.rotation * (_target.transform.position - transform.position);
        }

        _movement.Move(new Vector2(moveDirection.x, moveDirection.z));
    }
}
