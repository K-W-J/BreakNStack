using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.ScrollSnaps
{
    public class ScrollSnapMarker : MonoBehaviour
    {
        [SerializeField] private HorizontalScrollSnap scrollSnap;
        [Space]
        [ColorUsage(true, true)] [SerializeField] private Color selectColor;
        [ColorUsage(true, true)] [SerializeField] private Color defaultColor;
        [Space]
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject markPrefab;
        private List<Image> _markers;
        private int _beforeIndex;

        private void Awake()
        {
            for (int i = 0; i < content.childCount; i++)
                Instantiate(markPrefab, transform, false);
            
            _markers = GetComponentsInChildren<Image>()
                .Where(e => e != GetComponent<Image>()).ToList();
            
            foreach (var marker in _markers)
                marker.color = defaultColor;
            
            _markers.First().color = selectColor;
            
            scrollSnap.OnChildIndexUpdateEvent += ChangeCurrentIndex;
        }

        private void OnDestroy()
        {
            scrollSnap.OnChildIndexUpdateEvent -= ChangeCurrentIndex;
        }

        private void ChangeCurrentIndex(int index)
        {
            _markers[_beforeIndex].color = defaultColor;
            _markers[index].color = selectColor;
            _beforeIndex = index;
        }
    }
}