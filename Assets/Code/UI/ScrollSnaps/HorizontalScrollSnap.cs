using System.Collections.Generic;
using Settings.InputSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.UI.ScrollSnaps
{
    [RequireComponent(typeof(ScrollRect))]
    public class HorizontalScrollSnap : MonoBehaviour, IEndDragHandler, IBeginDragHandler
    {
        public UnityEvent<int> OnChildIndexUpdateEvent;
        
        [SerializeField] private PlayerInputSO playerInput;
        private List<RectTransform> _childrenTrm;
        private ScrollRect _scrollRect;
        
        private float _startPosX;
        private float _spacing;
        
        private int _currentIndex;
        
        private void Awake()
        {
            _scrollRect = GetComponent<ScrollRect>();
            _childrenTrm = new List<RectTransform>();

            _spacing = _scrollRect.content.GetComponent<HorizontalLayoutGroup>().spacing;
            
            foreach (RectTransform child in _scrollRect.content)
            {
                _childrenTrm.Add(child);
            }
            
            Debug.Assert(_childrenTrm.Count > 0, "childrenTrm is empty");
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            float deltaX = playerInput.PointDelta.x;
            float targetPosX = _childrenTrm[_currentIndex].rect.width + _spacing;

            bool canUpdatePos = false;

            if (deltaX > 0.1f && _currentIndex > 0)
            {
                --_currentIndex;
                canUpdatePos = true;
            }
            else if (deltaX < -0.1f && _currentIndex < _childrenTrm.Count - 1)
            {
                ++_currentIndex;
                targetPosX *= -1;
                canUpdatePos = true;
            }

            _scrollRect.StopMovement();
            _scrollRect.content.offsetMax = Vector2.zero;
            _scrollRect.content.offsetMin = new Vector2(_startPosX, _scrollRect.content.offsetMin.y);
            
            if (canUpdatePos)
            {
                print(targetPosX);
                print(_scrollRect.content.offsetMin.x);
                _scrollRect.content.offsetMin += new Vector2(targetPosX, 0);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _startPosX = _scrollRect.content.offsetMin.x;
        }
    }
}