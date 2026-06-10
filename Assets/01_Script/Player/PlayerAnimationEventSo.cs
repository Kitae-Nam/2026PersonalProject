using System;
using UnityEngine;

namespace _01_Script.Player
{
    [CreateAssetMenu(fileName = "PlayerAnimationEventSO", menuName = "SO/PlayerAnimation", order = 0)]
    public class PlayerAnimationEventSo : ScriptableObject
    {
        public Action<float> OnMove;
        public Action<HandType> OnContextChange;
        public Action OnTrigger;
        
        public void OnMoveInvoke(float normalizedTime)
        {
            OnMove?.Invoke(normalizedTime);
        }

        public void OnContextChangeInvoke(HandType handType)
        {
            OnContextChange?.Invoke(handType);
        }

        public void OnTriggerInvoke()
        {
            OnTrigger?.Invoke();
        }
    }
}