using Code.Core;
using Code.Etc;
using Code.Events;
using Settings.InputSystem;
using UnityEngine;

namespace Code.Blocks
{
    public class BlockDropper : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO blockEventChannel;
        [SerializeField] private PlayerInputSO playerInput;
        [SerializeField] private float dropBlockHeight;
        
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
            if(GameManager.Instance.IsStartGame == false || _currentBlock == null) return;
            
            float bottom = _camera.transform.position.y - _camera.orthographicSize;
            
            if (_isClicking)
            {
                float limitWidth = _camera.orthographicSize * _camera.aspect * 2;
                
                if (Mathf.Abs(playerInput.GetWorldPointPosition().x) < limitWidth)
                {
                    _currentBlock.transform.position = new Vector3(playerInput.GetWorldPointPosition().x, bottom + dropBlockHeight);
                }
            }
            else
            {
                _currentBlock.transform.position = new Vector3(0, bottom + dropBlockHeight);
            }
        }

        private void HandleDropBlock(bool isClicking)
        {
            if(GameManager.Instance.IsStartGame == false) return;
            
            _isClicking = isClicking;
            
            if(_currentBlock == null || isClicking) return;
            
            _currentBlock.DropBlock();
            _currentBlock = null;
            
            blockEventChannel.RaiseEvent(BlockEvents.BlockDropEvent.Initialize());
        }
        
        private void HandleCurrentBlock(BlockSpawnEvent evt)
        {
            _currentBlock = evt.block;
        }
    }
}