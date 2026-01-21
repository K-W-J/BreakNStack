using Code.Core;
using Code.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class QuitButton : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO uiEventChannel;
        
        private Button _quitButton;
        
        private void Awake()
        {
            _quitButton = GetComponent<Button>();
            _quitButton.onClick.AddListener(HandleClickqQuitBtn);
        }

        private void HandleClickqQuitBtn()
        {
            uiEventChannel.RaiseEvent(UIEvents.QuitGameEvent);
        }
    }
}