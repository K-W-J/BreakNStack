using System;
using System.Collections.Generic;
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
        private Vector2 _pointDelta;
        
        private Controls _controls;
        
        private PointerEventData _eventData;
        private List<RaycastResult> _results;
        
        private void OnEnable()
        {
            _results = new List<RaycastResult>();
                
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

        public void OnSwipe(InputAction.CallbackContext context)
        {
            _pointDelta = context.ReadValue<Vector2>();
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
            Debug.Assert(EventSystem.current != null, "No EventSystem.current in this scene.");
            
            _eventData ??= new PointerEventData(EventSystem.current);
            _results ??= new List<RaycastResult>();
    
            _eventData.position = _pointPosition; 

            _results.Clear();
            EventSystem.current.RaycastAll(_eventData, _results);

            return _results.Count > 0;
        }
    }
}