using Code.Core;
using Code.Etc;
using Code.Events;
using Code.Screens;
using GondrLib.Dependencies;
using InputSystem;
using UnityEngine;

namespace Code.Blocks
{
    public class BlockDropper : MonoBehaviour, IDependencyProvider
    {
        [SerializeField] private GameEventChannelSO blockEventChannel;
        [SerializeField] private PlayerInputSO playerInput;
        
        [SerializeField] private float dropBlockHeight;
        
        [Inject] private GameManager _gameManager;

        private Block _currentBlock;
        private bool _isClicking;
        
        private void Awake()
        {
            playerInput.OnDropPressed += HandleDropBlock;
            blockEventChannel.AddListener<BlockSpawnEvent>(HandleSpawnBlock);
        }

        private void OnDestroy()
        {
            playerInput.OnDropPressed -= HandleDropBlock;
            blockEventChannel.RemoveListener<BlockSpawnEvent>(HandleSpawnBlock);
        }

        private void Update()
        {
            if(_gameManager.IsPlayingGame == false || _currentBlock == null) return;
            
            if (_isClicking)
            {
                if (Mathf.Abs(playerInput.GetWorldPointPosition().x) < ScreenInfo.WidthScreen)
                {
                    _currentBlock.transform.position = new Vector3(playerInput.GetWorldPointPosition().x, ScreenInfo.BottomScreen + dropBlockHeight);
                }
                
            }
            else
            {
                _currentBlock.transform.position = new Vector3(0, ScreenInfo.BottomScreen + dropBlockHeight);
            }
        }

        private void HandleDropBlock(bool isClicking)
        {
            if (_gameManager.IsPlayingGame == false)
            {
                _isClicking = false;
                return;
            }
            
            _isClicking = isClicking;
            
            if (_currentBlock == null) return;

            if (isClicking == false)
            {
                _currentBlock.DropBlock();
                _currentBlock = null;
                blockEventChannel.RaiseEvent(BlockEvents.BlockDropEvent.Initialize());
            }
        }
        
        private void HandleSpawnBlock(BlockSpawnEvent evt)
        {
            _currentBlock = evt.block;
        }
    }
}