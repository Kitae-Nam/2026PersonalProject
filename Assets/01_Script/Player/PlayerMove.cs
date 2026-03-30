using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 8f;

    private Vector3 _movementDirection;
    private Rigidbody _rigid;


    private void Awake()
    {
        _rigid = GetComponent<Rigidbody>();
    }
    public void HandleMoveInput(Vector2 movementInput)
    {
        _movementDirection = new Vector3(movementInput.x, 0f, movementInput.y).normalized;
    }

    private void FixedUpdate()
    {
        _rigid.linearVelocity = _movementDirection * _moveSpeed;
        if (_movementDirection.sqrMagnitude > 0)
        {
            Quaternion rotation = Quaternion.LookRotation(_movementDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, _rotationSpeed * Time.fixedDeltaTime);
        }
    }
}
