using System;
using UnityEngine;

namespace _01_Script.Player
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private PlayerAnimationEventSo animationEvent;
        [SerializeField] private AnimatorOverrideController noneSet;
        [SerializeField] private AnimatorOverrideController holdingSet;
        [SerializeField] private AnimatorOverrideController equipSet;
        
        private Animator _animator;
        private HandType _handType =  HandType.None;
        
        static readonly int SpeedHash = Animator.StringToHash("Speed");
        static readonly int UseHash   = Animator.StringToHash("Use");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            animationEvent.OnMove +=  SetMoveSpeed;
            animationEvent.OnContextChange += SetContext;
            animationEvent.OnTrigger += TriggerUse;
        }

        private void OnDisable()
        {
            animationEvent.OnMove -=  SetMoveSpeed;
            animationEvent.OnContextChange -= SetContext;
            animationEvent.OnTrigger -= TriggerUse;
        }

        public void SetContext(HandType handType)
        {
            if (_handType == handType) return;
            _handType = handType;
            _animator.runtimeAnimatorController = _handType switch
            {
                HandType.None      => noneSet,
                HandType.Holding    => holdingSet,
                HandType.Equipment => equipSet,
                _ => noneSet
            };
        }
        public void SetMoveSpeed(float normalized) =>
            _animator.SetFloat(SpeedHash, normalized);

        public void TriggerUse()
        {
            if (_handType != HandType.Equipment) return;
            _animator.SetTrigger(UseHash);
        }
    }
}