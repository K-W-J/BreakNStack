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
        
        [SerializeField] private BlockSO blockSo;
        
        private Rigidbody2D _rigidbody;
        private SpriteRenderer _spriteRenderer;
        private Material _grayMat;
        private Camera _camera;
        
        private BlockState _blockState;
        
        private int _currentHealth;
        
        private bool IsMoveY => _rigidbody.linearVelocity.y == 0f;
        private bool IsLock => _blockState == BlockState.Lock;
        private bool _isGround;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            
            _grayMat = _spriteRenderer.material;
            _camera = Camera.main;
            
            _blockState = BlockState.None;
            
            _currentHealth = blockSo.maxHealth;
        }

        private void Update()
        {
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

            _rigidbody.linearDamping = 10f;
            
            int impactMagnitude = (int)collision.relativeVelocity.magnitude;
            
            if (collision.gameObject.TryGetComponent<Block>(out var block))
            {
                block.StopMove();
                block.TakeDamage(blockSo.weight + impactMagnitude);
            }
            
            StopMove();
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            _isGround = false;
            _rigidbody.linearDamping = 0f;
        }

        private void ChangeBreakSprite()
        {
            int healthQuarter = blockSo.maxHealth / 4;

            if (_currentHealth > healthQuarter * 3)
                _spriteRenderer.sprite = blockSo.default_Sprite;
            else if (_currentHealth > healthQuarter * 2)
                _spriteRenderer.sprite = blockSo.break_2_Sprite;
            else if (_currentHealth > healthQuarter)
                _spriteRenderer.sprite = blockSo.break_3_Sprite;
            else
                _spriteRenderer.sprite = blockSo.break_4_Sprite;
        }

        public void TakeDamage(int damage)
        {
            if(IsLock) return;
            
            OnDamageEvent?.Invoke();
            ChangeBreakSprite();
            
            _currentHealth -= damage;

            if (_currentHealth <= 0)
                DestroyBlock();
        }

        public void Heal(int heal)
        {
            ChangeBreakSprite();
            
            _currentHealth += heal;
            
            if (blockSo.maxHealth < _currentHealth)
                _currentHealth = blockSo.maxHealth;
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
                                * Random.Range(blockSo.minDistance, blockSo.maxDistance);//방향
            direction += Vector2.up * Random.Range(blockSo.minHeight, blockSo.maxHeight);//높이
            direction *= _rigidbody.mass;//질량 무시
            
            _rigidbody.gravityScale = 0.5f;
            _rigidbody.AddForce(direction, ForceMode2D.Impulse);
        }
        
        private void StopMove()
        {
            _rigidbody.linearVelocity = Vector2.zero;   
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
            
            /*GameObject[] blocks = _castChecker.GetCastData();
                
            foreach (GameObject block in blocks)
            {
                if (block != gameObject)
                {
                    IDamageable damageable = block.GetComponent<IDamageable>();
                        
                    if (damageable != null)
                        damageable.TakeDamage(weight);
                }
            }*/
        }

        private void DestroyBlock()
        {
            OnDestroyEvent?.Invoke();
            Destroy(gameObject);
        }
    }
}