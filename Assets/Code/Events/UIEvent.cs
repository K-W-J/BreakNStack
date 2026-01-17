using Code.Core;

namespace Code.Events
{
    public class UIEvent
    {
        public static CountTextEvent CountTextEvent = new CountTextEvent();
        public static PlayGameEvent PlayGameEvent = new PlayGameEvent();
    }
    
    public class PlayGameEvent : GameEvent
    {
        public PlayGameEvent Initialize()
        {
            return this;
        }
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
}