using System.Collections.Generic;
using Blade.Core;
using Code.Events;
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

        [SerializeField] private GameEventChannelSO blockEventChannel;
        [SerializeField] private float intensityDamage;
        [SerializeField] private bool canDamage;
        
        [Header("ResetBlock")]
        [SerializeField] private BlockSO blockData;
        
        private Rigidbody2D _rigidbody;
        private SpriteRenderer _spriteRenderer;
        private Material _grayMat;
        private Camera _camera;
        
        private List<Block> _adjacencyBlocks = new List<Block>();

        private BlockGuide _blockGuide;
        private GameObject _collider;

        private BlockState _blockState = BlockState.None;
        private int _currentHealth;
        
        private bool IsMove => Mathf.Abs(_rigidbody.linearVelocity.y) > 0.0001f 
                                || Mathf.Abs(_rigidbody.linearVelocity.x) > 0.0001f; //Approximately은 판정이 너무 작음
        public bool IsLock => _blockState == BlockState.Lock;

        public bool IsFirstTimeStack { get; private set; }
        private bool _isFirstTimeGround;

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
            _collider.transform.localScale = Vector3.one;
            
            float scaleX = _collider.transform.localScale.x * (blockData.isFlip ? -1: 1);
            Vector2 flipScale = new Vector2(scaleX, _collider.transform.localScale.y);
            _collider.transform.localScale = flipScale;
        }

        private void Awake()
        {
            blockEventChannel.AddListener<DestroyBlockEvent>(HandleDestroy);
            
            if(blockData != null)
                Initialize(blockData);
        }

        private void OnDestroy()
        {
            blockEventChannel.RemoveListener<DestroyBlockEvent>(HandleDestroy);
        }

        private void HandleDestroy(DestroyBlockEvent evt)
        {
            if(IsLock) return;
            
            if (_adjacencyBlocks.Contains(evt.block))
            {
                _adjacencyBlocks.Remove(evt.block);

                if (IsLock == false)
                {
                    SetFreezeAll(false);
                    SetForceDown();
                }
            }
        }

        public void Initialize(BlockSO blockSo)
        {
            if(blockData != null) return;
            
            blockData = blockSo;
            
            _camera = Camera.main;
            _rigidbody = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
                
            _rigidbody.mass = blockData.weight;
            _spriteRenderer.sprite = blockData.default_Sprite;
            _grayMat = _spriteRenderer.material;
            
            _collider = Instantiate(blockData.colliderPrefab, transform);
            _currentHealth = blockData.maxHealth;

            gameObject.name = $"{blockData.blockType.ToString()}_{blockData.blockName}_Block";
            _collider.transform.localScale = transform.localScale;
            SetFlip(blockData.isFlip);
            
            FireBlock();
        }

        private void Update()
        {
            if(IsLock) return;
            
            float limitLine = _camera.transform.position.y - (_camera.orthographicSize / 1.2f);
            
            if (_blockState == BlockState.Land && IsMove == false && limitLine > transform.position.y)
            {
                SetLockBlock(true);
            }
        }

        private void FixedUpdate()
        {
            if(IsLock || IsMove) return;
            
            if (_blockState == BlockState.Falling)
            {
                SetBlockStateToLand();
            }
            else if (_blockState == BlockState.Land)
            {
                SetFreezeAll(true);

                if (IsFirstTimeStack == false)
                {
                    blockEventChannel.RaiseEvent(BlockEvent.CoundBlockEvent.Initialize(blockData.stackCount));
                    IsFirstTimeStack = true;
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(IsLock) return;
            
            int impulseDamage = (int)collision.relativeVelocity.magnitude * 2;
            
            print(gameObject.name + " " + impulseDamage);
            
            if (collision.gameObject.TryGetComponent<Block>(out var block))
            {
                if (_adjacencyBlocks.Contains(block) == false)
                {
                    _adjacencyBlocks.Add(block);
                }
                
                SetFreezeAll(false);
                
                if(block.IsLock == false)
                    block.SetFreezeAll(false);

                if (_isFirstTimeGround == false)
                {
                    _isFirstTimeGround = true;
                    AddForceDown(5f);
                    block.AddForceDown(5f);
                }
                else
                {
                    AddForceDown();
                    block.AddForceDown();
                }

                if (impulseDamage > intensityDamage)
                    block.TakeDamage(impulseDamage / 2);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if(IsLock) return;
            
            if (collision.gameObject.TryGetComponent<Block>(out var block))
                if (_adjacencyBlocks.Contains(block))
                {
                    _adjacencyBlocks.Remove(block);
                    SetFreezeAll(false);
                    AddForceDown();
                }
        }

        private void SetFlip(bool isFlip)
        {
            _spriteRenderer.flipX = isFlip;
            _collider.transform.localScale = Vector3.one;
            
            float scaleX = _collider.transform.localScale.x * (blockData.isFlip ? -1: 1);
            Vector2 flipScale = new Vector2(scaleX, _collider.transform.localScale.y);
            _collider.transform.localScale = flipScale;
        }

        private void ChangeBreakSprite()
        {
            float healthRatio = ((float)_currentHealth / blockData.maxHealth) * 100;
            
            if (healthRatio > 75f)
                _spriteRenderer.sprite = blockData.default_Sprite;
            else if (healthRatio > 50f)
                _spriteRenderer.sprite = blockData.break_2_Sprite;
            else if (healthRatio > 25f)
                _spriteRenderer.sprite = blockData.break_3_Sprite;
            else
                _spriteRenderer.sprite = blockData.break_4_Sprite;
        }

        public void TakeDamage(int damage)
        {
            if(IsLock || canDamage == false) return;
            
            OnDamageEvent?.Invoke();
            
            _currentHealth -= damage;
            
            ChangeBreakSprite();

            if (_currentHealth <= 0)
            {
                blockEventChannel.RaiseEvent(BlockEvent.CoundBlockEvent.Initialize(blockData.destroyCount));
                DestroyBlock();
            }
        }

        public void Heal(int heal)
        {
            _currentHealth += heal;

            ChangeBreakSprite();
            
            if (blockData.maxHealth < _currentHealth)
                _currentHealth = blockData.maxHealth;
        }

        private void SetFreezeAll(bool isFreeze)
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
        
        private void SetForceDown(float addForce = 1f)
            => _rigidbody.linearVelocity = Vector2.down * addForce;
        private void AddForceDown(float addForce = 0.5f)
            => _rigidbody.AddForce(Vector2.down * addForce, ForceMode2D.Impulse);

        private void SetLockBlock(bool isLock)
        {
            float color = isLock ? 0 : 1f;
            _blockState = isLock ? BlockState.Lock : BlockState.None;
            _grayMat.SetFloat(Contrast, color);
            SetFreezeAll(isLock);
            
            if(isLock)
                blockEventChannel.RaiseEvent(BlockEvent.DestroyBlockEvent.Initialize(this));
        }

        [ContextMenu("DropBlock")]
        public void DropBlock()
        {
            SetForceDown();
            _rigidbody.gravityScale = 3f;
            
            _rigidbody.AddForce(Vector2.down * 1.5f, ForceMode2D.Force);
            _blockState = BlockState.Falling;
            
            _blockGuide.SetGuiding(false);
            _blockGuide = null;
        }
        
        private void SetBlockStateToLand()
        {
            _blockState = BlockState.Land;
            SetForceDown();
        }

        public void SetBlockGuide(BlockGuide blockGuide)
        {
            _blockGuide = blockGuide;
                
            _blockGuide.SetGuiding(true, transform);
            
            float scaleY = _blockGuide.transform.localScale.y / 2;
            Vector3 blockGuidePos = transform.position + Vector3.down * scaleY;
            _blockGuide.SetPosition(blockGuidePos);
            
            _blockGuide.SetScale(blockData.size);
        }
        
        [ContextMenu("DestroyBlock")]
        public void DestroyBlock()
        {
            if(_blockGuide != null)
                _blockGuide.SetGuiding(false);
            
            blockEventChannel.RaiseEvent(BlockEvent.DestroyBlockEvent.Initialize(this));
            
            OnDestroyEvent?.Invoke();
            Destroy(gameObject);
        }
    }
}