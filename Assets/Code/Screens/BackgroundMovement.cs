using Code.Core;
using Code.Events;
using UnityEngine;

namespace Code.Screens
{
    public class BackgroundMovement : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO uiEventChannel;
        [SerializeField] private float movementScale;
        
        private Camera _camera;
        
        private Vector2 _startPos;
        
        private void Awake()
        {
            _camera = Camera.main;
            _startPos = transform.position;
            
            uiEventChannel.AddListener<QuitGameEvent>(HandleQuitGame);
        }

        private void OnDestroy()
        {
            uiEventChannel.RemoveListener<QuitGameEvent>(HandleQuitGame);
        }

        private void Update()
        {
            Vector2 camPos = _camera.transform.position;
            transform.position = new Vector2(transform.position.x, camPos.y * movementScale);
        }
        
        private void HandleQuitGame(QuitGameEvent evt)
        {
            transform.position = _startPos;
        }
    }
}