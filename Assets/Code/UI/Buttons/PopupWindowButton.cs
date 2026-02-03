using Code.Define;
using Code.Events;
using UnityEngine;

namespace Code.UI.Buttons
{
    public class PopupWindowButton : ButtonBase, IPopWindowButton
    {
        [field:SerializeField] public WindowType WindowType { get; private set; }
        
        protected override void Awake()
        {
            base.Awake();
            
            Button.onClick.AddListener(HandleClickPauseBtn);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            Button.onClick.RemoveListener(HandleClickPauseBtn);
        }
        
        private void HandleClickPauseBtn()
        {
            uiEventChannel.RaiseEvent(UIEvents.OpenWindowEvent.Initialize(WindowType));
            uiEventChannel.RaiseEvent(UIEvents.PauseGameEvent);
        }
    }
}