using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Settings.InputSystem
{
    [CreateAssetMenu(fileName = "PlayerInputSO", menuName = "SO/PlayerInput", order = 0)]
    public class PlayerInputSO : ScriptableObject, Controls.IPlayerActions
    {
        public event Action<bool> OnDropPressed;
        
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
            if(IsPointerOverUI()) return;
            
            if(context.started)
                OnDropPressed?.Invoke(true);
            else if(context.canceled)
                OnDropPressed?.Invoke(false);
            
        }

        public void OnPointPosition(InputAction.CallbackContext context)
        {
            _pointPosition = context.ReadValue<Vector2>();
        }
        
        public Vector2 GetWorldPointPosition()
        {
            Camera mainCamera = Camera.main;
            Debug.Assert(mainCamera != null, "No main camera in this scene.");

            if (IsPointerOverUI())
                return Vector2.zero;
            
            return mainCamera.ScreenToWorldPoint(_pointPosition);
        }
        
        private bool IsPointerOverUI()
        {
            Debug.Assert(EventSystem.current != null, "No EventSystem in this scene.");
            return EventSystem.current.IsPointerOverGameObject();
        }
    }
}