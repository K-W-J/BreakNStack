using Code.Core;

namespace Code.Events
{
    public class UIEvents
    {
        public static CountTextEvent CountTextEvent = new CountTextEvent();
        public static PlayGameEvent PlayGameEvent = new PlayGameEvent();
        public static StopGameEvent StopGameEvent = new StopGameEvent();
        public static ResetGameEvent ResetGameEvent = new ResetGameEvent();
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
    
    public class PlayGameEvent : GameEvent
    { }
    
    public class StopGameEvent : GameEvent
    { }
    
    public class ResetGameEvent : GameEvent
    { }
}