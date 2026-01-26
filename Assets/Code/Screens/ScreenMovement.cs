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
        [SerializeField] private Transform heightMark;
        [SerializeField] private float screenLineHeight;
        [Space]
        [SerializeField] private float speed;
        [SerializeField] private bool isAutoMove;
        
        private List<Block> _blockList;
        private Camera _camera;
        
        private Vector2 _startPos;
        private float _posY;
        
        private void Awake()
        {
            _blockList = new List<Block>();
            _camera = Camera.main;
            
            _startPos = transform.position;
            _posY = _startPos.y;
            
            blockEventChannel.AddListener<BlockPushEvent>(HandleRemoveBlockList);
            blockEventChannel.AddListener<BlockLandEvent>(HandleLandBlock);
            uiEventChannel.AddListener<QuitGameEvent>(HandleQuitGame);
        }

        private void OnDestroy()
        {
            blockEventChannel.RemoveListener<BlockPushEvent>(HandleRemoveBlockList);
            blockEventChannel.RemoveListener<BlockLandEvent>(HandleLandBlock);
            uiEventChannel.RemoveListener<QuitGameEvent>(HandleQuitGame);
        }
        
        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(0, _posY, -10), Time.deltaTime * speed);
            heightMark.position = new Vector3(0, _posY, -10);
        }

        private void MoveScreen()
        {
            if (_blockList.Count < 1) return;
            
            float topScreen = _camera.transform.position.y + _camera.orthographicSize;
            
            _blockList.Sort((a, b) =>
                (b.transform.position.y - topScreen)
                .CompareTo(a.transform.position.y - topScreen));
            
            float firstBlockPosY = _blockList.First().transform.position.y;
            
            _posY = firstBlockPosY + screenLineHeight;
        }

        private void HandleLandBlock(BlockLandEvent evt)
        {
            if (_blockList.Contains(evt.block) == false)
                _blockList.Add(evt.block);

            MoveScreen();
        }

        private void HandleRemoveBlockList(BlockPushEvent evt)
        {
            if (_blockList.Contains(evt.block))
                _blockList.Remove(evt.block);

            MoveScreen();
        }
        
        private void HandleQuitGame(QuitGameEvent evt)
        {
            _posY = _startPos.y;
            transform.position = _startPos;
        }
    }
}