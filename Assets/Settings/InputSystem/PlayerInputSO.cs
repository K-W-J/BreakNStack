using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Settings.InputSystem
{
    [CreateAssetMenu(fileName = "PlayerInputSO", menuName = "SO/PlayerInput", order = 0)]
    public class PlayerInputSO : ScriptableObject, Controls.IPlayerActions
    {
        public event Action<bool> OnDropPressed;
        
        private Camera _camera;
        private Controls _controls;
        
        private void OnEnable()
        {
            _camera = Camera.main;
            
            Debug.Assert(_camera != null, "No main camera in this scene.");
            
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
            if(context.started)
                OnDropPressed?.Invoke(true);
            else if(context.canceled)
                OnDropPressed?.Invoke(false);
        }
        
        public Vector2 GetWorldMousePosition()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            return _camera.ScreenToWorldPoint(new Vector2(mousePosition.x, mousePosition.y));
        }
    }
}