using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Blocks
{
    internal class BlockSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject[] blockPrefab;
        
        private Transform[] _blockSpawnPoint;
        private Transform CurrentSpawnPoint =>
            _blockSpawnPoint[Random.Range(0, _blockSpawnPoint.Length)];
        
        private Block _currentBlock;

        private float _spawnTimer;
        private float _currentTime;

        private void Awake()
        {
            _blockSpawnPoint = GetComponentsInChildren<Transform>().Where(t => t != transform).ToArray();
            _spawnTimer = 2f;
        }

        private void Update()
        {
            _currentTime += Time.deltaTime;
            
            if(_spawnTimer < _currentTime && _currentBlock == null)
            {
                SpawnBlock();
                _currentTime = 0;
            }
        }

        public void DropBlock()
        {
            if (_currentBlock != null)
            {
                _currentBlock.SetBlockStateToFalling();
                _currentBlock = null;
            }
        }
        
        private void SpawnBlock()
        {
            int rand = Random.Range(0, blockPrefab.Length);
            GameObject blockObject = Instantiate(blockPrefab[rand], CurrentSpawnPoint.position, Quaternion.identity);
            _currentBlock = blockObject.GetComponent<Block>();
            _currentBlock.Initialize();
        }
    }
}
