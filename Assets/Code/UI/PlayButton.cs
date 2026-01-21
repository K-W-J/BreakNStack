using Code.Core;
using Code.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class PlayButton : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO uiEventChannel;
        
        private Button _playButton;
        
        private void Awake()
        {
            _playButton = GetComponent<Button>();
            _playButton.onClick.AddListener(HandleClickPlayBtn);
        }

        private void HandleClickPlayBtn()
        {
            uiEventChannel.RaiseEvent(UIEvents.PlayGameEvent);
        }
    }
}