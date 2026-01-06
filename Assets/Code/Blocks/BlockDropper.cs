using System;
using Code.Core;
using Code.Events;
using Settings.InputSystem;
using UnityEngine;

namespace Code.Blocks
{
    public class BlockDropper : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO blockEventChannel;
        [SerializeField] private PlayerInputSO playerInput;
        
        private Camera _camera;
        private Block _currentBlock;
        private bool _isClicking;
        
        private void Awake()
        {
            _camera = Camera.main;
            playerInput.OnDropPressed += HandleDropBlock;
            blockEventChannel.AddListener<BlockSpawnEvent>(HandleCurrentBlock);
        }
        
        private void OnDestroy()
        {
            playerInput.OnDropPressed -= HandleDropBlock;
            blockEventChannel.RemoveListener<BlockSpawnEvent>(HandleCurrentBlock);
        }

        private void Update()
        {
            if (_isClicking && _currentBlock != null)
            {
                float limitWidth = _camera.orthographicSize * _camera.aspect * 2;
                
                if (Mathf.Abs(playerInput.GetWorldPointPosition().x) < limitWidth)
                {
                    _currentBlock.transform.position = new Vector3(playerInput.GetWorldPointPosition().x, _currentBlock.transform.position.y);
                }
            }
        }

        private void HandleDropBlock(bool isClicking)
        {
            _isClicking = isClicking;
            
            if(_currentBlock == null || isClicking) return;
            
            _currentBlock.DropBlock();
            _currentBlock = null;
            
            blockEventChannel.RaiseEvent(BlockEvent.BlockDropEvent.Initialize());
        }
        
        private void HandleCurrentBlock(BlockSpawnEvent evt)
        {
            _currentBlock = evt.block;
        }
    }
}