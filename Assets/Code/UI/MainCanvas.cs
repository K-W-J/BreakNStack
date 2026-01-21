using Code.Core;
using Code.Events;
using UnityEngine;

namespace Code.UI
{
    public class MainCanvas : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO uiEventChannel;
        [Space]
        [SerializeField] private Canvas inGameCanvas; 
        [SerializeField] private Canvas menuCanvas;

        private void Awake()
        {
            uiEventChannel.AddListener<PlayGameEvent>(HandlePlayGame);
        }

        private void OnDestroy()
        {
            uiEventChannel.RemoveListener<PlayGameEvent>(HandlePlayGame);
        }
        
        private void HandlePlayGame(PlayGameEvent evt)
        {
            menuCanvas.gameObject.SetActive(false);
            inGameCanvas.gameObject.SetActive(true);
        }
    }
}