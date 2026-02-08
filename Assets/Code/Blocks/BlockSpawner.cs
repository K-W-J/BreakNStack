using Code.Core;
using Code.Etc;
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
        [SerializeField] private GameEventChannelSO uiEventChannel;
        [Space]
        [SerializeField] private BlockSO[] blockData;
        [SerializeField] private GhostBlock ghostBlock;
        [SerializeField] private ScreenMovement screenMovement;
        [SerializeField] private Transform blockSpawnPoint;
        [Space]
        [SerializeField] private float spawnDelay;
        
        private Block _currentBlock;
        
        private float _currentSpawnDelay;
        
        private bool CanSpawn => _currentBlock == null || _currentBlock.IsLand || _currentBlock.IsDead;

        private void Awake()
        {
            uiEventChannel.AddListener<PlayGameEvent>(HandlePlayGame);
        }

        private void OnDestroy()
        {
            uiEventChannel.RemoveListener<PlayGameEvent>(HandlePlayGame);
        }
        
        private void HandlePlayGame(PlayGameEvent evt)
        {
            if(CanSpawn == false) return;
            SpawnBlock();
        }
        
        private void Update()
        {
            if (GameManager.Instance.IsPlayingGame == false) return;
            
            if (_currentSpawnDelay > spawnDelay && CanSpawn)
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
            //_currentBlock.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f);
            
            int rand = Random.Range(0, blockData.Length);
            _currentBlock.InitializeSpawn(blockData[rand]);
            
            blockEventChannel.RaiseEvent(BlockEvents.BlockSpawnEvent.Initialize(_currentBlock));
        }
    }
}
