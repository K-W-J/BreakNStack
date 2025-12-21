using System;
using UnityEngine;

namespace Code.Blocks
{
    internal class BlockGenerator : MonoBehaviour
    {
        [SerializeField] private Transform blockSpawnPoint;
        [SerializeField] private GameObject blockPrefab;
        public Block CurrentBlock { get; private set; }

        private float _spawnTimer;
        private float _currentTime;

        private void Awake()
        {
            _spawnTimer = 2f;
        }

        private void Update()
        {
            _currentTime += Time.deltaTime;
            
            if(_spawnTimer < _currentTime)
            {
                SpawnBlock();
                _currentTime = 0;
            }
        }
        
        public void SpawnBlock()
        {
            GameObject blockObject = Instantiate(blockPrefab, transform.position, Quaternion.identity);
            CurrentBlock = blockObject.GetComponent<Block>();
        }

        public void DropBlock()
        {
            if (CurrentBlock != null)
            {
                CurrentBlock.SetBlockStateToFalling();
            }
        }
    }
}
