using UnityEngine;

namespace Code.Screens
{
    public class ScreenInfo : MonoBehaviour
    {
        private static Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
            Debug.Assert(_mainCamera != null, "Camera not found");
        }

        public static float TopScreen => _mainCamera.transform.position.y + _mainCamera.orthographicSize;
        public static float BottomScreen => _mainCamera.transform.position.y - _mainCamera.orthographicSize;
        public static float WidthScreen => _mainCamera.orthographicSize * _mainCamera.aspect;
        public static float HeightScreen => _mainCamera.orthographicSize;
        public static Vector2 ScreenPosition => _mainCamera.transform.position;
        public static Camera MainCamera => _mainCamera;
    }
}