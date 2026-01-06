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
        [Space]
        [SerializeField] private Transform screenLine;
        [SerializeField] private float screenLineHeight;
        [Space]
        [SerializeField] private float speed;
        [SerializeField] private bool isAutoMove;
        
        private List<Block> _blockList;
        private Camera _camera;
        
        private float _posY;
        
        private void OnValidate()
        {
            if(screenLine == null) return;
            
            float posY = Camera.main.transform.position.y + Camera.main.orthographicSize;
            screenLine.position = new Vector3(0, posY - screenLineHeight, -10);
        }
        private void Awake()
        {
            _blockList = new List<Block>();
            _camera = Camera.main;
            _posY = transform.position.y;
            
            blockEventChannel.AddListener<BlockPushEvent>(HandleRemoveBlockList);
            blockEventChannel.AddListener<BlockSpawnEvent>(HandleAddBlockList);
            blockEventChannel.AddListener<BlockLandEvent>(HandleLandBlock);
        }

        private void OnDestroy()
        {
            blockEventChannel.RemoveListener<BlockPushEvent>(HandleRemoveBlockList);
            blockEventChannel.RemoveListener<BlockSpawnEvent>(HandleAddBlockList);
            blockEventChannel.RemoveListener<BlockLandEvent>(HandleLandBlock);
        }
        
        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(0, _posY, -10), Time.deltaTime * speed);
        }
        
        private void HandleAddBlockList(BlockSpawnEvent evt)
        {
            if (_blockList.Contains(evt.block) == false)
                _blockList.Add(evt.block);
        }
        
        private void HandleLandBlock(BlockLandEvent evt)
        {
            if (_blockList.Count < 1) return;
            
            float topScreen = _camera.transform.position.y + _camera.orthographicSize;
            float topLinePosY = topScreen - screenLineHeight;
            
            float firstBlockPosY = _blockList.First().transform.position.y;
            
            _blockList.Sort((a, b) =>
                (b.transform.position.y - topScreen)
                .CompareTo(a.transform.position.y - topScreen));
            
            if (topLinePosY < firstBlockPosY)
                _posY = transform.position.y + (firstBlockPosY - topLinePosY);
        }

        private void HandleRemoveBlockList(BlockPushEvent evt)
        {
            if (_blockList.Contains(evt.block))
                _blockList.Remove(evt.block);
        }
    }
}