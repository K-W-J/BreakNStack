using UnityEngine;

namespace Code.Screens
{
    public class ScreenInfo : MonoBehaviour
    {
        public static float TopScreen
        {
            get
            {
                Camera mainCamera = Camera.main;
                if (mainCamera == null)
                {
                    Debug.Assert(false, "Camera not found");
                    return 0;
                }
                return mainCamera.transform.position.y + mainCamera.orthographicSize;
            }
        }
        
        public static float BottomScreen
        {
            get
            {
                Camera mainCamera = Camera.main;
                if (mainCamera == null)
                {
                    Debug.Assert(false, "Camera not found");
                    return 0;
                }
                return mainCamera.transform.position.y - mainCamera.orthographicSize;
            }
        }
        
        public static float WidthScreen
        {
            get
            {
                Camera mainCamera = Camera.main;
                if (mainCamera == null)
                {
                    Debug.Assert(false, "Camera not found");
                    return 0;
                }
                return mainCamera.orthographicSize * mainCamera.aspect * 2;
            }
        }
    }
}