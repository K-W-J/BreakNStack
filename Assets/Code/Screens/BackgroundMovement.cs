using UnityEngine;

namespace Code.Screens
{
    public class BackgroundMovement : MonoBehaviour
    {
        [SerializeField] private float movementScale;
        private Camera _camera;
        
        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            Vector2 camPos = _camera.transform.position;
            transform.position = new Vector2(transform.position.x, camPos.y * movementScale);
        }
    }
}