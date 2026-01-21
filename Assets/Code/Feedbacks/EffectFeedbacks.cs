using Code.Blocks;
using Code.Core;
using Code.Events;
using Code.Modules;
using UnityEngine;

namespace Code.Feedbacks
{
    public class EffectFeedback : Feedback, IModule
    {
        [SerializeField] private GameEventChannelSO effectChannel;
        
        private Block _block;
        public void InitializeComponent(ModuleOwner owner)
        {
            _block = owner as Block;
        }
        
        public override void CreateFeedback()
        {
            effectChannel.RaiseEvent(EffectEvents.PlayEffectEvent.Initialize(_block.BlockData.effectPoolItem,transform.position));
        }

        public override void StopFeedback()
        {
            
        }
    }
}