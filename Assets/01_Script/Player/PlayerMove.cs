using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;

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
    }
}
