using UnityEngine;

namespace Code.Screens
{
    public class ScreenResolution : MonoBehaviour
    {
        private Camera _camera;
        private void Awake()
        {
            _camera = Camera.main;
            
            
        }
    }
}