using UnityEngine;

namespace _01_Script.Player
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] private PlayerAnimationEventSo animationEvent;
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _rotationSpeed = 8f;
        [SerializeField] private PlayerAnimation _playerAnimation;
        [SerializeField] private ParticleSystem _footstepParticle;

        private Vector3 _movementDirection;
        private Rigidbody _rigid;
        private bool _isMoving; 

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

            bool isMovingNow = _movementDirection.sqrMagnitude > 0f;
            animationEvent.OnMoveInvoke(isMovingNow ? 1f : 0f);

            // 상태가 바뀌는 순간에만 파티클 제어
            if (isMovingNow && _isMoving == false)
            {
                _footstepParticle.Play();
            }
            else if (isMovingNow == false && _isMoving)
            {
                _footstepParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
            _isMoving = isMovingNow;

            if (isMovingNow)
            {
                Quaternion rotation = Quaternion.LookRotation(_movementDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, _rotationSpeed * Time.fixedDeltaTime);
            }
        }
    }
}
