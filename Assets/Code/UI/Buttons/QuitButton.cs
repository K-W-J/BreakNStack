using Code.Events;

namespace Code.UI.Buttons
{
    public class QuitButton : ButtonBase
    {
        protected override void Awake()
        {
            base.Awake();
            
            Button.onClick.AddListener(HandleClickqQuitBtn);
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            Button.onClick.RemoveListener(HandleClickqQuitBtn);
        }

        private void HandleClickqQuitBtn()
        {
            uiEventChannel.RaiseEvent(UIEvents.QuitGameEvent);
        }
    }
}