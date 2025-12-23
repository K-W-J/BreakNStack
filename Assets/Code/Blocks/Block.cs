using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Code.Blocks
{
    public enum BlockState
    {
        None,
        Fire,
        Falling,
        Land,
        Lock,
    }
    
    public class Block : MonoBehaviour, IDamageable
    {
        private static readonly int Contrast = Shader.PropertyToID("_Contrast");
        
        public UnityEvent OnDestroyEvent;
        public UnityEvent OnDamageEvent;
        
        [Header("ResetBlock")]
        [SerializeField] private BlockSO blockData;
        
        private Rigidbody2D _rigidbody;
        private SpriteRenderer _spriteRenderer;
        private Material _grayMat;
        private Camera _camera;

        private GameObject _collider;
        
        private BlockState _blockState = BlockState.None;
        private int _currentHealth;
        
        private bool IsMoveY => _rigidbody.linearVelocity.y == 0f;
        private bool IsLock => _blockState == BlockState.Lock;
        private bool _isGround;

        [ContextMenu("ResetBlock")]
        private void ResetBlock()
        {
            if (blockData == null)
            {
                Debug.Log($"Block SO가 없습니다.");
                return;
            }
            
            gameObject.name = $"{blockData.blockType.ToString()}_{blockData.blockName}_Block";      
            SetFlip(blockData.isFlip);
            
            if(_collider != null)
                DestroyImmediate(_collider);
            
            _collider = Instantiate(blockData.colliderPrefab, transform);
            GetComponentInChildren<SpriteRenderer>().sprite = blockData.default_Sprite;
        }

        private void Awake()
        {
            if(blockData != null)
                Initialize(blockData);
        }

        public void Initialize(BlockSO blockSo)
        {
            if(blockData != null) return;
            
            blockData = blockSo;
            
            _camera = Camera.main;
            _rigidbody = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
                
            _spriteRenderer.sprite = blockData.default_Sprite;
            _grayMat = _spriteRenderer.material;
            
            _collider = Instantiate(blockData.colliderPrefab, transform);
            _currentHealth = blockData.maxHealth;

            gameObject.name = $"{blockData.blockType.ToString()}_{blockData.blockName}_Block";
            SetFlip(blockData.isFlip);
            
            FireBlock();
        }

        private void Update()
        {
            if(_blockState == BlockState.Lock) return;
            
            float limitLine = _camera.transform.position.y - (_camera.orthographicSize / 1.2f);
            
            if (_blockState == BlockState.Land && IsMoveY && limitLine > transform.position.y)
            {
                SetLockBlock(true);
            }
        }

        private void FixedUpdate()
        {
            if (_blockState == BlockState.Falling && IsMoveY)
            {
                SetBlockStateToLand();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(IsLock || _isGround) return;
            
            _isGround = true;
            
            int impactMagnitude = (int)collision.relativeVelocity.magnitude;
            
            if(impactMagnitude < 5) return;
            
            _rigidbody.linearDamping = 10f;
            
            if (collision.gameObject.TryGetComponent<Block>(out var block))
            {
                block.StopMove();
                block.TakeDamage(blockData.weight + impactMagnitude);
            }
            
            StopMove();
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            _isGround = false;
            _rigidbody.linearDamping = 0f;
        }

        private void SetFlip(bool isFlip)
        {
            _spriteRenderer.flipX = isFlip;
            float scaleX = isFlip ? -transform.localScale.x : transform.localScale.x;
            Vector2 flipScale = new Vector2(scaleX, transform.localScale.y);
            _collider.transform.localScale = flipScale;
        }

        private void ChangeBreakSprite()
        {
            int healthQuarter = blockData.maxHealth / 4;

            if (_currentHealth > healthQuarter * 3)
                _spriteRenderer.sprite = blockData.default_Sprite;
            else if (_currentHealth > healthQuarter * 2)
                _spriteRenderer.sprite = blockData.break_2_Sprite;
            else if (_currentHealth > healthQuarter)
                _spriteRenderer.sprite = blockData.break_3_Sprite;
            else
                _spriteRenderer.sprite = blockData.break_4_Sprite;
        }

        public void TakeDamage(int damage)
        {
            if(IsLock) return;
            
            OnDamageEvent?.Invoke();
            
            _currentHealth -= damage;
            
            ChangeBreakSprite();

            if (_currentHealth <= 0)
                DestroyBlock();
        }

        public void Heal(int heal)
        {
            _currentHealth += heal;

            ChangeBreakSprite();
            
            if (blockData.maxHealth < _currentHealth)
                _currentHealth = blockData.maxHealth;
        }

        private void SetFreezePosition(bool isFreeze)
        {
            if(isFreeze)
                _rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            else
                _rigidbody.constraints = RigidbodyConstraints2D.None;
        }
        
        public void FireBlock()
        {
            _blockState = BlockState.Fire;
            
            Vector2 direction = (transform.position.x > 0 ? Vector2.left : Vector2.right)
                                * Random.Range(blockData.minDistance, blockData.maxDistance);//방향
            direction += Vector2.up * Random.Range(blockData.minHeight, blockData.maxHeight);//높이
            direction *= _rigidbody.mass;//질량 무시
            
            _rigidbody.gravityScale = 0.5f;
            _rigidbody.AddForce(direction, ForceMode2D.Impulse);
        }
        
        private void StopMove()
        {
            _rigidbody.linearVelocity = Vector2.down;   
        }

        private void SetLockBlock(bool isLock)
        {
            if (isLock)
            {
                _grayMat.SetFloat(Contrast, 0f);
                SetFreezePosition(true);
                _blockState = BlockState.Lock;
            }
            else
            {
                _grayMat.SetFloat(Contrast, 1f);
                SetFreezePosition(false);
                _blockState = BlockState.None;
            }
        }

        public void SetBlockStateToFalling()
        {
            _rigidbody.linearVelocity = Vector2.down;
            _rigidbody.gravityScale = 3f;
            
            _rigidbody.AddForce(Vector2.down * 1.5f, ForceMode2D.Force);
            _blockState = BlockState.Falling;
            SetFreezePosition(false);
        }
        
        private void SetBlockStateToLand()
        {
            _blockState = BlockState.Land;
            StopMove();
        }

        public void DestroyBlock()
        {
            OnDestroyEvent?.Invoke();
            Destroy(gameObject);
        }
    }
}