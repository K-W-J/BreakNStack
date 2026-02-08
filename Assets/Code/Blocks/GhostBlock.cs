using Code.Core;
using Code.Events;
using UnityEngine;

namespace Code.Blocks
{
    public class GhostBlock : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO blockEventChannel;
        [Space]
        [SerializeField] private LayerMask whatIsTarget;
        [SerializeField] private float distance;
        [Space]
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        private Block _dropBlock;
        
        private void Awake()
        {
            blockEventChannel.AddListener<BlockSpawnEvent>(HandleBlockSpawn);
            blockEventChannel.AddListener<BlockDropEvent>(HandleBlockDrop);
            
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            blockEventChannel.RemoveListener<BlockSpawnEvent>(HandleBlockSpawn);
            blockEventChannel.RemoveListener<BlockDropEvent>(HandleBlockDrop);
        }

        private void HandleBlockDrop(BlockDropEvent evt)
        {
            gameObject.SetActive(false);
        }

        private void HandleBlockSpawn(BlockSpawnEvent evt)
        {
            _dropBlock = evt.block;
            
            gameObject.SetActive(true);
            
            spriteRenderer.sprite = _dropBlock.BlockData.default_Sprite;
            spriteRenderer.flipX = _dropBlock.BlockData.isFlip;
        }

        private void Update()
        {
            if(_dropBlock == null) return;

            if (TryGetHitPoint(out var hit, _dropBlock.transform.position, _dropBlock.BlockData.size))
            {
                float blockHalfHeight = hit.point.y + _dropBlock.BlockData.size.y * 0.4f;
                transform.position = new Vector3(_dropBlock.transform.position.x, blockHalfHeight, -10);
                
                spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            }
            else
            {
                spriteRenderer.color = new Color(1, 1, 1, 0);
            }
        }

        public bool TryGetHitPoint(out RaycastHit2D hit, Vector2 position, Vector2 boxSize)
        {
            hit = Physics2D.BoxCast(position, boxSize * 0.8f,
                0f, transform.up, distance, whatIsTarget);
            return hit.collider != null;
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if(_dropBlock == null) return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawRay(_dropBlock.transform.position, transform.up * distance);

            Gizmos.color = Color.yellow;
            Gizmos.matrix = _dropBlock.transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.up * distance, _dropBlock.BlockData.size);
        }
#endif
    }
}
