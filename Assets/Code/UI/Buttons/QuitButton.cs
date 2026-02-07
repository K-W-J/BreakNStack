using Code.Events;

namespace Code.UI.Buttons
{
    public class QuitButton : ButtonBase
    {
        protected override void Awake()
        {
            base.Awake();
            
            Button.onClick.AddListener(HandleClickQuitBtn);
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            Button.onClick.RemoveListener(HandleClickQuitBtn);
        }

        private void HandleClickQuitBtn()
        {
            uiEventChannel.RaiseEvent(UIEvents.QuitGameEvent);
        }
    }
}