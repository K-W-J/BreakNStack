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
        private List<Block> _movingBlocks;
        private bool _isQuitGame;

        private void Awake()
        {
            _blocks = new List<Block>();
            _movingBlocks = new List<Block>();
            
            blockChannel.AddListener<BlockSpawnEvent>(HandleBlockSpawn);
            blockChannel.AddListener<BlockPushEvent>(HandleBlockPush);
            blockChannel.AddListener<BlockMoveEvent>(HandleBlockMove);
            blockChannel.AddListener<BlockStopEvent>(HandleBlockStop);
            uiChannel.AddListener<QuitGameEvent>(HandleQuitGame);
        }
        
        private void OnDestroy()
        {
            blockChannel.RemoveListener<BlockSpawnEvent>(HandleBlockSpawn);
            blockChannel.RemoveListener<BlockPushEvent>(HandleBlockPush);
            blockChannel.RemoveListener<BlockMoveEvent>(HandleBlockMove);
            blockChannel.RemoveListener<BlockStopEvent>(HandleBlockStop);
            uiChannel.RemoveListener<QuitGameEvent>(HandleQuitGame);
        }

        private void HandleBlockMove(BlockMoveEvent evt)
        {
            if (_movingBlocks.Contains(evt.block) == false)
            {
                _movingBlocks.Add(evt.block);
            }
        }
        
        private void HandleBlockStop(BlockStopEvent evt)
        {
            if(_isQuitGame) return;
            
            if (_movingBlocks.Contains(evt.block))
            {
                _movingBlocks.Remove(evt.block);
            }
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