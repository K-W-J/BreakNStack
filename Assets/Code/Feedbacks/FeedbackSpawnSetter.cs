using System.Collections.Generic;
using System.Linq;
using Code.Blocks;
using Code.Modules;
using UnityEngine;

namespace Code.Feedbacks
{
    public class FeedbackSpawnSetter : MonoBehaviour, IModule, IInitializeSpawn
    {
        List<IFeedbackSettable> _feedbackSettables;
        
        private Block _block;
        
        public void InitializeComponent(ModuleOwner owner)
        {
            _block = owner as Block;
            Debug.Assert(_block != null, "FeedbackManager should be attached to Block");
            
            _feedbackSettables = GetComponents<IFeedbackSettable>().ToList();    
        }

        public void InitializeSpawn()
        {
            foreach (var feedbackSettable in _feedbackSettables)
                feedbackSettable.SetFeedback(_block.BlockData);
        }
    }
}