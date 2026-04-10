using System.Collections.Generic;
using System.Linq;
using Code.Blocks;
using Code.Core;
using Code.Events;
using UnityEngine;

namespace Code.Screens
{
    public class ScreenMovement : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO blockEventChannel;
        [SerializeField] private GameEventChannelSO uiEventChannel;
        [Space] 
        [SerializeField] private GameObject panel;
        [SerializeField] private Transform trackingTarget;
        [SerializeField] private float screenLineHeight;
        [Space]
        [SerializeField] private float speed;
        [SerializeField] private bool isAutoMove;
        
        public bool IsMoveScreen => Mathf.Approximately(transform.position.y, _posY);
        
        private List<Block> _blockList;
        private List<Block> _movingBlocks;
        
        private Vector2 _startPos;
        private float _posY;
        
        private void Awake()
        {
            _blockList = new List<Block>();
            _movingBlocks = new List<Block>();
            
            _startPos = transform.position;
            _posY = _startPos.y;
            
            blockEventChannel.AddListener<BlockPushEvent>(HandleRemoveBlockList);
            blockEventChannel.AddListener<BlockLandEvent>(HandleLandBlock);
            blockEventChannel.AddListener<BlockMoveEvent>(HandleBlockMove);
            blockEventChannel.AddListener<BlockStopEvent>(HandleBlockStop);
            uiEventChannel.AddListener<QuitGameEvent>(HandleQuitGame);
        }

        private void OnDestroy()
        {
            blockEventChannel.RemoveListener<BlockPushEvent>(HandleRemoveBlockList);
            blockEventChannel.RemoveListener<BlockLandEvent>(HandleLandBlock);
            blockEventChannel.RemoveListener<BlockMoveEvent>(HandleBlockMove);
            blockEventChannel.RemoveListener<BlockStopEvent>(HandleBlockStop);
            uiEventChannel.RemoveListener<QuitGameEvent>(HandleQuitGame);
        }

        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(0, _posY, -10), Time.deltaTime * speed);
            trackingTarget.position = new Vector3(0, _posY, -10);
        }
        
        private void FixedUpdate()
        {
            if (isAutoMove == false) return;
            
            if (_movingBlocks.Count > 0) return;
            
            if (_blockList.Count <= 0) return;
            
            MoveScreen(_blockList.First().transform.position);
        }

        private void MoveScreen(Vector2 targetPos)
        {
            _posY = targetPos.y + screenLineHeight;
        }
        
        private void HandleBlockMove(BlockMoveEvent evt)
        {
            if (_movingBlocks.Contains(evt.block) == false)
                _movingBlocks.Add(evt.block);

            DistanceSort(_movingBlocks);
            
            if (_movingBlocks.Count < 5 || _movingBlocks.First().MovingVelocity < 2f) return;
            
            panel.SetActive(true);
            
            MoveScreen(_movingBlocks[^1].transform.position);
        }
        
        private void HandleBlockStop(BlockStopEvent evt)
        {
            if (_movingBlocks.Contains(evt.block))
                _movingBlocks.Remove(evt.block);
            
            panel.SetActive(false);
        }

        private void HandleLandBlock(BlockLandEvent evt)
        {
            if (_blockList.Contains(evt.block) == false)
                _blockList.Add(evt.block);
            
            DistanceSort(_blockList);
            
            if (_blockList.Count < 1) return;

            MoveScreen(_blockList.First().transform.position);
        }

        private void HandleRemoveBlockList(BlockPushEvent evt)
        {
            if (_blockList.Contains(evt.block))
                _blockList.Remove(evt.block);
            
            if (_movingBlocks.Contains(evt.block))
                _movingBlocks.Remove(evt.block);

            DistanceSort(_blockList);
            
            if (_blockList.Count < 1) return;
            
            panel.SetActive(false);

            MoveScreen(_blockList.First().transform.position);
        }
        
        private void HandleQuitGame(QuitGameEvent evt)
        {
            _posY = _startPos.y;
            transform.position = _startPos;
        }

        private void DistanceSort(List<Block> blocks)
        {
            blocks.Sort((a, b) =>
                (b.transform.position.y - ScreenInfo.TopScreen)
                .CompareTo(a.transform.position.y - ScreenInfo.TopScreen));
        }
    }
}