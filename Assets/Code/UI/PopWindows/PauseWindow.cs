using Code.Events;

namespace Code.UI.PopWindows
{
    public class PauseWindow : PopWindow
    {
        protected override void HandleOffWindow()
        {
            base.HandleOffWindow();
            uiEventChannel.RaiseEvent(UIEvents.PlayGameEvent);
        }
    }
}   