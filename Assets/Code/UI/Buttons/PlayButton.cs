using Code.Events;

namespace Code.UI.Buttons
{
    public class PlayButton : ButtonBase
    {
        protected override void Awake()
        {
            base.Awake();
            
            Button.onClick.AddListener(HandleClickPlayBtn);
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            Button.onClick.RemoveListener(HandleClickPlayBtn);
        }

        private void HandleClickPlayBtn()
        {
            uiEventChannel.RaiseEvent(UIEvents.PlayGameEvent);
        }
    }
}