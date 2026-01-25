using Code.Core;
using Code.Define;

namespace Code.Events
{
    public class UIEvents
    {
        public static CountTextEvent CountTextEvent = new CountTextEvent();
        public static PlayGameEvent PlayGameEvent = new PlayGameEvent();
        public static QuitGameEvent QuitGameEvent = new QuitGameEvent();
        public static PauseGameEvent PauseGameEvent = new PauseGameEvent();
        public static ResetGameEvent ResetGameEvent = new ResetGameEvent();
        public static OpenWindowEvent OpenWindowEvent = new OpenWindowEvent();
        public static CloseWindowEvent CloseWindowEvent = new CloseWindowEvent();
    }
    
    public class CountTextEvent : GameEvent
    {
        public int count;

        public CountTextEvent Initialize(int count)
        {
            this.count = count;
            return this;
        }
    }
    
    public class OpenWindowEvent : GameEvent
    {
        public WindowType windowType;

        public OpenWindowEvent Initialize(WindowType windowType)
        {
            this.windowType = windowType;
            return this;
        }
    }
    
    public class CloseWindowEvent : GameEvent
    {
        public WindowType windowType;

        public CloseWindowEvent Initialize(WindowType windowType)
        {
            this.windowType = windowType;
            return this;
        }
    }
    
    public class PlayGameEvent : GameEvent
    { }    
    
    public class QuitGameEvent : GameEvent
    { }
    
    public class PauseGameEvent : GameEvent
    { }
    
    public class ResetGameEvent : GameEvent
    { }
}