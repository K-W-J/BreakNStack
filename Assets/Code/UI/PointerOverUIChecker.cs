using System.Collections.Generic;
using GondrLib.Dependencies;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Code.UI
{
    [Provide]
    public class PointerOverUIChecker : MonoBehaviour, IDependencyProvider
    {
        [SerializeField] private GraphicRaycaster graphicRaycaster;
        
        private List<RaycastResult> _results;
        private EventSystem _eventSystem;

        private void Awake()
        {
            _results = new List<RaycastResult>();
            _eventSystem = EventSystem.current;
        }

        public bool IsPointerOverUI()
        {
            if (_eventSystem == null || graphicRaycaster == null)
                return false;

            PointerEventData pointerData = new PointerEventData(_eventSystem)
            {
                position = Pointer.current.position.ReadValue()
            };

            _results.Clear();
            graphicRaycaster.Raycast(pointerData, _results);
            
            return _results.Count > 0;
        }
    }
}