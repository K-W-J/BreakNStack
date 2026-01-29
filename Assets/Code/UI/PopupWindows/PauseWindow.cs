using Code.Events;

namespace Code.UI.PopupWindows
{
    public class PauseWindow : PopupWindow
    {
        protected override void HandleOffWindow()
        {
            base.HandleOffWindow();
            uiEventChannel.RaiseEvent(UIEvents.PlayGameEvent);
        }
    }
}   