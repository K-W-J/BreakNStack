using Blade.Core;
using UnityEngine;

namespace Code.Events
{
    public class EffectEvent
    {
        public static PlayEffectEvent PlayEffectEvent = new PlayEffectEvent();
    }
    
    public class PlayEffectEvent : GameEvent
    {
        public Vector2 position;

        public PlayEffectEvent Initialize(Vector2 position)
        {
            this.position = position;
            return this;
        }
    }
}