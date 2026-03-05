using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private PlayerInputSO _playerInput;
    [SerializeField] private float _moveSpeed = 5f;

    private Vector3 _movementDirection;
    private Rigidbody _rigid;

    private void Awake()
    {
        _playerInput.OnMovementChange += HandleMoveInput;
        _rigid = GetComponent<Rigidbody>();
    }
    private void OnDestroy()
    {
        _playerInput.OnMovementChange -= HandleMoveInput;
    }

    private void HandleMoveInput(Vector2 movementInput)
    {
        _movementDirection = new Vector3(movementInput.x, 0f, movementInput.y).normalized;
    }

    private void FixedUpdate()
    {
        _rigid.linearVelocity = _movementDirection * _moveSpeed;
    }
}
