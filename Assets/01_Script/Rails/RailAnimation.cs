using System;
using UnityEngine;

namespace _01_Script.Rails
{
    public class RailAnimation : MonoBehaviour
    {
        public Action OnRailMade;
        
        [SerializeField] private Animator animator;
        private readonly int _triggerHash = Animator.StringToHash("RailMade");

        private void OnEnable()
        {
            animator = GetComponentInChildren<Animator>();
            OnRailMade += AnimationRail;
        }

        private void OnDisable()
        {
            OnRailMade -= AnimationRail;
        }

        private void AnimationRail()
        {
            if (animator != null)
            {
                animator.SetTrigger(_triggerHash);
            }
        }
    }
}