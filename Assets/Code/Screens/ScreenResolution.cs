using UnityEngine;

namespace Code.Screens
{
    public class ScreenResolution : MonoBehaviour
    {
        [SerializeField] private Vector2 resolution;
        
        private Camera _camera;
        
        private void Awake()
        {
            _camera = Camera.main;
            
            Debug.Assert(_camera != null, "not found MainCamera");
            
            Rect rect = _camera.rect;
            float scaleHeight = ((float)Screen.width / Screen.height ) / (resolution.x / resolution.y);
            float scaleWidth = 1f / scaleHeight;

            if (scaleHeight < 1f)
            {
                rect.height = scaleHeight;
                rect.y = (1f - scaleHeight) / 2f;
            }
            else
            {
                rect.width = scaleWidth;
                rect.x = (1f - scaleWidth) / 2f;
            }
            
            _camera.rect = rect;
        }
    }
}