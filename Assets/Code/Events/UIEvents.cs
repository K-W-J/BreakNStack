using Code.Core;
using Code.Defines;

namespace Code.Events
{
    public class UIEvents
    {
        public static SetComboTimeEvent SetComboTimeEvent = new SetComboTimeEvent();
        public static ComboScoreTextEvent ComboScoreTextEvent = new ComboScoreTextEvent();
        public static ScoreTextEvent ScoreTextEvent = new ScoreTextEvent();
        public static HighScoreTextEvent HighScoreTextEvent = new HighScoreTextEvent();
        
        public static PlayGameEvent PlayGameEvent = new PlayGameEvent();
        public static QuitGameEvent QuitGameEvent = new QuitGameEvent();
        public static DangerImageEvent DangerImageEvent = new DangerImageEvent();
        public static PauseGameEvent PauseGameEvent = new PauseGameEvent();
        public static ResetGameEvent ResetGameEvent = new ResetGameEvent();
        public static OpenWindowEvent OpenWindowEvent = new OpenWindowEvent();
        public static CloseWindowEvent CloseWindowEvent = new CloseWindowEvent();
    }
        
    public class SetComboTimeEvent : GameEvent
    {
        public float time;

        public SetComboTimeEvent Initialize(float time)
        {
            this.time = time;
            return this;
        }
    }
    
    public class ComboScoreTextEvent : GameEvent
    {
        public int score;

        public ComboScoreTextEvent Initialize(int score)
        {
            this.score = score;
            return this;
        }
    }
    
    public class ScoreTextEvent : GameEvent
    {
        public int score;

        public ScoreTextEvent Initialize(int score)
        {
            this.score = score;
            return this;
        }
    }
    
    public class HighScoreTextEvent : GameEvent
    {
        public int highScore;

        public HighScoreTextEvent Initialize(int highScore)
        {
            this.highScore = highScore;
            return this;
        }
    }
    
    public class DangerImageEvent : GameEvent
    {
        public DangerImageEvent Initialize(int score)
        {
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