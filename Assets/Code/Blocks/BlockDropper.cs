using Code.Core;
using Code.Etc;
using Code.Events;
using Code.Screens;
using GondrLib.Dependencies;
using Settings.InputSystem;
using UnityEngine;

namespace Code.Blocks
{
    public class BlockDropper : MonoBehaviour, IDependencyProvider
    {
        [SerializeField] private GameEventChannelSO blockEventChannel;
        [SerializeField] private PlayerInputSO playerInput;
        [SerializeField] private float dropBlockHeight;

        public Block CurrentBlock { get; private set; }
        
        private Camera _camera;
        private bool _isClicking;
        
        private void Awake()
        {
            _camera = Camera.main;
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
            if(GameManager.Instance.IsPlayingGame == false || CurrentBlock == null) return;
            
            if (_isClicking)
            {
                if (Mathf.Abs(playerInput.GetWorldPointPosition().x) < ScreenInfo.WidthScreen)
                {
                    CurrentBlock.transform.position = new Vector3(playerInput.GetWorldPointPosition().x, ScreenInfo.BottomScreen + dropBlockHeight);
                }
            }
            else
            {
                CurrentBlock.transform.position = new Vector3(0, ScreenInfo.BottomScreen + dropBlockHeight);
            }
        }

        private void HandleDropBlock(bool isClicking)
        {
            if (GameManager.Instance.IsPlayingGame == false)
            {
                _isClicking = false;
                return;
            }
            
            _isClicking = isClicking;

            if (isClicking == false && CurrentBlock != null)
            {
                CurrentBlock.DropBlock();
                CurrentBlock = null;
                blockEventChannel.RaiseEvent(BlockEvents.BlockDropEvent.Initialize());
            }
        }
        
        private void HandleSpawnBlock(BlockSpawnEvent evt)
        {
            CurrentBlock = evt.block;
        }
    }
}