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
            uiEventChannel.AddListener<QuitGameEvent>(HandleQuitGame);
        }

        private void OnDestroy()
        {
            uiEventChannel.RemoveListener<PlayGameEvent>(HandlePlayGame);
            uiEventChannel.RemoveListener<QuitGameEvent>(HandleQuitGame);
        }

        private void HandleQuitGame(QuitGameEvent evt)
        {
            ChangeCanvas(false);
        }

        private void HandlePlayGame(PlayGameEvent evt)
        {
            ChangeCanvas(true);
        }

        private void ChangeCanvas(bool isPlay)
        {
            menuCanvas.gameObject.SetActive(!isPlay);
            inGameCanvas.gameObject.SetActive(isPlay);
        }
    }
}