using UnityEngine;

namespace _01_Script.Player
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] private PlayerAnimationEventSo animationEvent;
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _rotationSpeed = 8f;
        [SerializeField] private PlayerAnimation _playerAnimation;

        private Vector3 _movementDirection;
        private Rigidbody _rigid;

        private void Awake()
        {
            _rigid = GetComponent<Rigidbody>();
            _playerAnimation = GetComponentInChildren<PlayerAnimation>();
        }
        public void HandleMoveInput(Vector2 movementInput)
        {
            _movementDirection = new Vector3(movementInput.x, 0f, movementInput.y).normalized;
        }

        private void FixedUpdate()
        {
            _rigid.linearVelocity = _movementDirection * _moveSpeed;
            animationEvent.OnMoveInvoke(_movementDirection.sqrMagnitude > 0f ? 1f : 0f);
            if (_movementDirection.sqrMagnitude > 0)
            {
                Quaternion rotation = Quaternion.LookRotation(_movementDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, _rotationSpeed * Time.fixedDeltaTime);
            }
        }
    }
}
