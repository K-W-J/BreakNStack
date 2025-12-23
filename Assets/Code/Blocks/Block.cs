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
        
        private bool IsMoveY => Mathf.Abs(_rigidbody.linearVelocity.y) < 0.0001f; //Approximately은 판정이 너무 작음
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
            
            _spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
            _spriteRenderer.sprite = blockData.default_Sprite;
            _spriteRenderer.flipX = blockData.isFlip;
            
            if(_collider != null)
                DestroyImmediate(_collider);
            
            _collider = Instantiate(blockData.colliderPrefab, transform);
            float scaleX = blockData.isFlip ? -transform.localScale.x : transform.localScale.x;
            Vector2 flipScale = new Vector2(scaleX, transform.localScale.y);
            _collider.transform.localScale = flipScale;
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
            
            int impulseDamage = (int)collision.relativeVelocity.magnitude * 2;
            
            print(gameObject.name + impulseDamage);

            if (impulseDamage < 5)
            {
                _rigidbody.linearVelocity /= 2f;
                return;
            }
            
            if (collision.gameObject.TryGetComponent<Block>(out var block))
            {
                block.StopMove();
                block.TakeDamage(impulseDamage);
            }
            
            StopMove();
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
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
            float healthRatio = (float)_currentHealth / blockData.maxHealth;
            
            if (healthRatio > 0.75f)
                _spriteRenderer.sprite = blockData.default_Sprite;
            else if (healthRatio > 0.5f)
                _spriteRenderer.sprite = blockData.break_2_Sprite;
            else if (healthRatio > 0.25f)
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