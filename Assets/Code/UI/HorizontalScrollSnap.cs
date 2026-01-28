using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.UI
{
    public class HorizontalScrollSnap : MonoBehaviour, IPointerUpHandler
    {
        private List<RectTransform> _childrenTrm;
        private ScrollRect _scrollRect;
        
        private void Awake()
        {
            _childrenTrm = new List<RectTransform>(GetComponentsInChildren<RectTransform>());   
            _scrollRect = GetComponentInParent<ScrollRect>();
            
            Debug.Assert(_childrenTrm.Count > 0, "childrenTrm is empty");
            Debug.Assert(_scrollRect != null, "scrollRect is null");
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            print(_scrollRect.horizontalNormalizedPosition);
        }
    }
}