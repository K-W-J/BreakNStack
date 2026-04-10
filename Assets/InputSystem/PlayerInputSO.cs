using System;
using Code.Screens;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace InputSystem
{
    [CreateAssetMenu(fileName = "PlayerInputSO", menuName = "SO/PlayerInput", order = 0)]
    public class PlayerInputSO : ScriptableObject, Controls.IPlayerActions
    {
        public event Action<bool> OnDropPressed;

        public Vector2 PointDelta { get; private set; }
        private Vector2 _pointPosition;
        
        private Controls _controls;
        
        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new Controls();
                _controls.Player.SetCallbacks(this);
            }
            
            _controls.Player.Enable();
        }

        private void OnDisable()
        {
            _controls.Player.Disable();
        }

        public void OnDrop(InputAction.CallbackContext context)
        {
            if (context.performed) 
            {
                OnDropPressed?.Invoke(true);
            }
            else if (context.canceled) 
            {
                OnDropPressed?.Invoke(false);
            }
        }

        public void OnPointPosition(InputAction.CallbackContext context)
        {
            _pointPosition = context.ReadValue<Vector2>();
        }

        public void OnSwipe(InputAction.CallbackContext context)
        {
            PointDelta = context.ReadValue<Vector2>();
        }

        public Vector2 GetWorldPointPosition()
        {
            return ScreenInfo.MainCamera.ScreenToWorldPoint(_pointPosition);
        }
    }
}