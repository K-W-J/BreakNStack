using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Blocks
{
    internal class BlockSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject blockPrefab;
        [Space]
        [SerializeField] private BlockSO[] blockData;
        [SerializeField] private BlockGuide blockGuide;
        
        private Transform[] _blockSpawnPoint;
        private Transform RandomSpawnPoint =>
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
            blockGuide.gameObject.SetActive(false);
            blockGuide.transform.SetParent(transform);
            
            if (_currentBlock != null)
            {
                _currentBlock.DropBlock();
                _currentBlock = null;
            }
        }
        
        private void SpawnBlock()
        {
            GameObject blockObject = Instantiate(blockPrefab, RandomSpawnPoint.position, Quaternion.identity);
            _currentBlock = blockObject.GetComponent<Block>();
            
            int rand = Random.Range(0, blockData.Length);
            _currentBlock.Initialize(blockData[rand]);
            
            blockGuide.gameObject.SetActive(true);
            blockGuide.transform.SetParent(_currentBlock.transform);
            blockGuide.transform.position = _currentBlock.transform.position;
            
            float scaleY = blockGuide.transform.localScale.y / 2;
            blockGuide.transform.position += Vector3.down * scaleY;
            blockGuide.SetScale(blockData[rand].size);
        }
    }
}
