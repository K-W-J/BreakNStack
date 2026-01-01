using UnityEngine;

namespace Code
{
    public class BackgroundMovement : MonoBehaviour
    {
        [SerializeField] private float speed;
        private Camera _camera;
        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            Vector2 camPos = _camera.transform.position;
            transform.position = new Vector2(transform.position.x, camPos.y * speed);
        }
    }
}