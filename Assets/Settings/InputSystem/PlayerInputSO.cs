using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Settings.InputSystem
{
    [CreateAssetMenu(fileName = "PlayerInputSO", menuName = "SO/PlayerInput", order = 0)]
    public class PlayerInputSO : ScriptableObject, Controller.IPlayerActions
    {
        public event Action OnDropPressed;
        
        private Controller _controller;
        
        private void OnEnable()
        {
            if (_controller == null)
            {
                _controller = new Controller();
                _controller.Player.SetCallbacks(this);
            }
            
            _controller.Player.Enable();
        }

        private void OnDisable()
        {
            _controller.Player.Disable();
        }

        public void OnDrop(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnDropPressed?.Invoke();
        }
    }
}