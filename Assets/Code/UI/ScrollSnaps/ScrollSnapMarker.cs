using UnityEngine;

namespace Code.UI.ScrollSnaps
{
    public class ScrollSnapMarker : MonoBehaviour
    {
        private int _currentIndex;
        
        public void ChangeCurrentIndex(int index)
        {
            _currentIndex = index;
        }
    }
}