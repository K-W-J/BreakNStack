using System.Linq;
using Blade.Core;
using Code.Core;
using Code.Events;
using Code.Screens;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Blocks
{
    internal class BlockSpawner : MonoBehaviour
    {
        [SerializeField] private PoolItemSO blockItem;
        [SerializeField] private GameEventChannelSO blockEventChannel;
        [Space]
        [SerializeField] private BlockSO[] blockData;
        [SerializeField] private BlockGuide blockGuide;
        [SerializeField] private ScreenMovement screenMovement;
        [Space]
        [SerializeField] private float spawnTimer;
        
        private Transform[] _blockSpawnPoint;
        private Transform RandomSpawnPoint =>
            _blockSpawnPoint[Random.Range(0, _blockSpawnPoint.Length)];
        
        private Block _currentBlock;
        
        private float _currentTime;
        private bool _isFallingCurrentBlock;
        private bool _isCurrentBlockLand;

        private void Awake()
        {
            blockEventChannel.AddListener<BlockLandEvent>(HandleLandBlock);
            _blockSpawnPoint = GetComponentsInChildren<Transform>().Where(t => t != transform).ToArray();
        }

        private void OnDestroy()
        {
            blockEventChannel.RemoveListener<BlockLandEvent>(HandleLandBlock);
        }

        private void HandleLandBlock(BlockLandEvent evt)
        {
            _isCurrentBlockLand = true;
        }

        private void Update()
        {
            if (_currentTime > spawnTimer && (_isCurrentBlockLand || _currentBlock == null))
            {
                SpawnBlock();
                _isCurrentBlockLand = false;
                _isFallingCurrentBlock = false;
                _currentTime = 0;
            }
            else
            {
                _currentTime += Time.deltaTime;
            }
            
            if (_currentBlock != null && _currentBlock.IsDead)
            {
                _currentBlock = null;
                _isFallingCurrentBlock = false;
            }
        }

        public void DropBlock()
        {
            if (_currentBlock != null && _isFallingCurrentBlock == false)
            {
                _currentBlock.DropBlock();
                _isFallingCurrentBlock = true;
            }
        }
        
        private void SpawnBlock()
        {
            _currentBlock = PoolManager.Instance.Pop<Block>(blockItem);
            _currentBlock.transform.SetPositionAndRotation(RandomSpawnPoint.position, Quaternion.identity);
            
            blockEventChannel.RaiseEvent(BlockEvent.BlockSpawnEvent.Initialize(_currentBlock));
            
            int rand = Random.Range(0, blockData.Length);
            _currentBlock.InitializeSpawn(blockData[rand]);
            _currentBlock.SetBlockGuide(blockGuide);
        }
    }
}
