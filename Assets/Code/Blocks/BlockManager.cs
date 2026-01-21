using System.Collections.Generic;
using Code.Core;
using Code.Events;
using UnityEngine;

namespace Code.Blocks
{
    public class BlockManager : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO blockChannel;
        [SerializeField] private GameEventChannelSO uiChannel;
        private List<Block> _blocks;
        private bool _isQuitGame;

        private void Awake()
        {
            _blocks = new List<Block>();
            
            blockChannel.AddListener<BlockSpawnEvent>(HandleBlockSpawn);
            blockChannel.AddListener<BlockPushEvent>(HandleBlockPush);
            uiChannel.AddListener<QuitGameEvent>(HandleQuitGame);
        }
        
        private void OnDestroy()
        {
            blockChannel.RemoveListener<BlockSpawnEvent>(HandleBlockSpawn);
            blockChannel.RemoveListener<BlockPushEvent>(HandleBlockPush);
            uiChannel.RemoveListener<QuitGameEvent>(HandleQuitGame);
        }

        private void HandleBlockSpawn(BlockSpawnEvent evt)
        {
            if (_blocks.Contains(evt.block) == false)
            {
                _blocks.Add(evt.block);
            }
        }
        
        private void HandleBlockPush(BlockPushEvent evt)
        {
            if(_isQuitGame) return;
            
            if (_blocks.Contains(evt.block))
            {
                _blocks.Remove(evt.block);
            }
        }
        
        private void HandleQuitGame(QuitGameEvent evt)
        {
            if(_blocks.Count <= 0) return;
            
            _isQuitGame = true;
            
            foreach (var block in _blocks)
            {
                block.PushBlock();
            }
            
            _blocks.Clear();
            
            _isQuitGame = false;
        }
    }
}