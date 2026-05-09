using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _01_Script.Player
{
    [CreateAssetMenu(fileName = "PlayerInputSO", menuName = "SO/PlayerInputSO")]
    public class PlayerInputSO : ScriptableObject, Controls.IPlayerActions
    {
        public event Action<Vector2> OnMovementChange;
        public event Action OnAttackChange;
        public event Action OnInteractionChange;

        private Controls controls;

        private void OnEnable()
        {
            if(controls == null)
            {
                controls = new Controls();
                controls.Player.SetCallbacks(this);
            }
            controls.Player.Enable();
        }

        private void OnDisable()
        {
            controls.Player.Disable();
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                OnAttackChange?.Invoke();
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 movement = context.ReadValue<Vector2>();
            OnMovementChange?.Invoke(movement);
        }

        public void OnInteraction(InputAction.CallbackContext context)
        {
            if(context.started)
            {
                OnInteractionChange?.Invoke();
            }
        }
    }
}
