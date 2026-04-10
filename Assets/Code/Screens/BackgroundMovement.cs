using Code.Core;
using Code.Events;
using UnityEngine;

namespace Code.Screens
{
    public class BackgroundMovement : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO uiEventChannel;
        [Space]
        [SerializeField] private float movementScale;
        
        private Vector2 _startPos;
        
        private void Awake()
        {
            _startPos = transform.position;
            
            uiEventChannel.AddListener<QuitGameEvent>(HandleQuitGame);
        }

        private void OnDestroy()
        {
            uiEventChannel.RemoveListener<QuitGameEvent>(HandleQuitGame);
        }

        private void Update()
        {
            transform.position = new Vector2(transform.position.x, ScreenInfo.ScreenPosition.y * movementScale);
        }
        
        private void HandleQuitGame(QuitGameEvent evt)
        {
            transform.position = _startPos;
        }
    }
}