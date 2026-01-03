using Blade.Core;
using Code.Events;
using UnityEngine;

namespace Code.FeedBacks
{
    public class EffectFeedBack : FeedBack
    {
        [SerializeField] private GameEventChannelSO effectChannel;

        public override void CreateFeedback()
        {
            effectChannel.RaiseEvent(EffectEvent.PlayEffectEvent.Initialize(transform.position));
        }

        public override void StopFeedback()
        {
        }
    }
}