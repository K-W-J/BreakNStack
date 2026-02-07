using UnityEngine;

namespace Code.Blocks
{
    public class GhostBlock : MonoBehaviour
    {
        [SerializeField] private LayerMask whatIsTarget;
        [SerializeField] private float distance;
        [Space]
        [SerializeField] private SpriteRenderer spriteRenderer; 
        [SerializeField] private Block block;
        
        public void SetActive(bool isGuiding) => gameObject.SetActive(isGuiding);

        public void Initialize(Sprite sprite, Vector2 scale, Vector2 position)
        {
            spriteRenderer.sprite = sprite;
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            
            SetActive(true);
            SetScaleAndPosition(scale, position);
        }

        private void Update()
        {
            
        }

        public Vector2 GetHitPoint(Vector2 position, Vector2 boxSize)
        {
            RaycastHit2D hit = Physics2D.BoxCast(position, boxSize * 0.5f,
                0f, transform.up, distance, whatIsTarget);
            Debug.Assert(hit.collider != null, nameof(hit.collider) + " != null");
            return hit.point;
        }
        
        public void SetScaleAndPosition(Vector2 scale, Vector2 position)
        {
            transform.localScale = new Vector2(scale.x, scale.y);
            transform.position = position;
        }
    }
}
