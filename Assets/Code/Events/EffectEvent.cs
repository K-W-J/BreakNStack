using Code.Core;
using UnityEngine;

namespace Code.Events
{
    public class EffectEvent
    {
        public static PlayEffectEvent PlayEffectEvent = new PlayEffectEvent();
    }
    
    public class PlayEffectEvent : GameEvent
    {
        public PoolItemSO effectItem;
        public Vector2 position;

        public PlayEffectEvent Initialize(PoolItemSO effectItem, Vector2 position)
        {
            this.effectItem = effectItem;
            this.position = position;
            return this;
        }
    }
}