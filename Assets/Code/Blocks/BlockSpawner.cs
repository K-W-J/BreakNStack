using System;
using System.Linq;
using Blade.Core;
using Code.Core;
using Code.Events;
using Code.Screens;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Blocks
{
    public class BlockSpawner : MonoBehaviour
    {
        [SerializeField] private PoolItemSO blockItem;
        [SerializeField] private GameEventChannelSO blockEventChannel;
        [Space]
        [SerializeField] private BlockSO[] blockData;
        [SerializeField] private BlockGuide blockGuide;
        [SerializeField] private ScreenMovement screenMovement;
        [SerializeField] private Transform blockSpawnPoint;
        [Space]
        [SerializeField] private float spawnTimer;
        
        private Block _currentBlock;
        
        private float _currentTime;

        private void Start()
        {
            SpawnBlock();
        }

        private void Update()
        {
            if (_currentTime > spawnTimer && (_currentBlock.IsLand || _currentBlock.IsDead))
            {
                SpawnBlock();
                _currentTime = 0;
            }
            else
            {
                _currentTime += Time.deltaTime;
            }
        }
        
        private void SpawnBlock()
        {
            _currentBlock = PoolManager.Instance.Pop<Block>(blockItem);
            _currentBlock.transform.SetPositionAndRotation(blockSpawnPoint.position, Quaternion.identity);
            
            blockEventChannel.RaiseEvent(BlockEvent.BlockSpawnEvent.Initialize(_currentBlock));
            
            int rand = Random.Range(0, blockData.Length);
            _currentBlock.InitializeSpawn(blockData[rand]);
            _currentBlock.SetBlockGuide(blockGuide);
        }
    }
}
