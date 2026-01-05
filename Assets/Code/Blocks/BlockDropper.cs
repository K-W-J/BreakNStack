using System;
using Blade.Core;
using Code.Events;
using Settings.InputSystem;
using UnityEngine;

namespace Code.Blocks
{
    public class BlockDropper : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO blockEventChannel;
        [SerializeField] private PlayerInputSO playerInput;
        
        private Block _currentBlock;
        
        private void Awake()
        {
            playerInput.OnDropPressed += OnDrop;
            blockEventChannel.AddListener<BlockSpawnEvent>(HandleCurrentBlock);
        }

        private void HandleCurrentBlock(BlockSpawnEvent evt)
        {
            _currentBlock = evt.block;
        }

        private void OnDestroy()
        {
            playerInput.OnDropPressed -= OnDrop;
        }

        private void OnDrop()
        {
            _currentBlock.DropBlock();
        }

        private void Update()
        {
            
        }
    }
}