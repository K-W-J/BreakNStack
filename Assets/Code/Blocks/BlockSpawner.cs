using System;
using System.Linq;
using Code.Core;
using Code.Events;
using Code.Screens;
using DG.Tweening;
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
        [SerializeField] private float spawnDelay;
        
        private Block _currentBlock;
        
        private float _currentSpawnDelay;

        private void Start()
        {
            SpawnBlock();
        }

        private void Update()
        {
            if (_currentSpawnDelay > spawnDelay && (_currentBlock.IsLand || _currentBlock.IsDead))
            {
                SpawnBlock();
                _currentSpawnDelay = 0;
            }
            else
            {
                _currentSpawnDelay += Time.deltaTime;
            }
        }
        
        private void SpawnBlock()
        {
            _currentBlock = PoolManager.Instance.Pop<Block>(blockItem);
            _currentBlock.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f);
            
            blockEventChannel.RaiseEvent(BlockEvent.BlockSpawnEvent.Initialize(_currentBlock));
            
            int rand = Random.Range(0, blockData.Length);
            _currentBlock.InitializeSpawn(blockData[rand]);
            _currentBlock.SetBlockGuide(blockGuide);
        }
    }
}
