using System.Collections.Generic;
using Blade.Core;
using Code.Agents;
using Code.Events;
using UnityEngine;
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
    
    public class Block : Agent, IDamageable
    {
        private static readonly int Contrast = Shader.PropertyToID("_Contrast");

        [SerializeField] private GameEventChannelSO blockEventChannel;
        [SerializeField] private float intensityDamage;

        [Header("ResetBlock")]
        [field:SerializeField] public BlockSO BlockData { get; private set; }

        private Rigidbody2D _rigidbody;
        private SpriteRenderer _spriteRenderer;
        private Material _grayMat;
        private Camera _camera;
        
        private List<Block> _adjacencyBlocks;

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
            if (BlockData == null)
            {
                Debug.Log($"Block SO가 없습니다.");
                return;
            }
            
            gameObject.name = $"{BlockData.blockType.ToString()}_{BlockData.blockName}_Block";      
            
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _spriteRenderer.sprite = BlockData.default_Sprite;
            _spriteRenderer.flipX = BlockData.isFlip;

            if(_collider != null)
                DestroyImmediate(_collider);
            
            _collider = Instantiate(BlockData.colliderPrefab, transform);
            _collider.transform.localScale = Vector3.one;
            
            float scaleX = _collider.transform.localScale.x * (BlockData.isFlip ? -1: 1);
            Vector2 flipScale = new Vector2(scaleX, _collider.transform.localScale.y);
            _collider.transform.localScale = flipScale;
        }

        protected override void Awake()
        {
            base.Awake();
            
            _camera = Camera.main;
            _rigidbody = GetComponentInChildren<Rigidbody2D>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            
            _adjacencyBlocks = new List<Block>();
            
            blockEventChannel.AddListener<DestroyBlockEvent>(HandleDestroy);
            
            if(BlockData != null)
                Initialize(BlockData);
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
            if(BlockData != null) return;
            
            BlockData = blockSo;

            _rigidbody.mass = BlockData.weight;
            _spriteRenderer.sprite = BlockData.default_Sprite;
            _grayMat = _spriteRenderer.material;
            
            _collider = Instantiate(BlockData.colliderPrefab, transform);
            _currentHealth = BlockData.maxHealth;

            gameObject.name = $"{BlockData.blockType.ToString()}_{BlockData.blockName}_Block";
            _collider.transform.localScale = transform.localScale;
            SetFlip(BlockData.isFlip);
            
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
                    blockEventChannel.RaiseEvent(BlockEvent.CoundBlockEvent.Initialize(BlockData.stackCount));
                    IsFirstTimeStack = true;
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(IsLock) return;
            
            int impulseDamage = (int)collision.relativeVelocity.magnitude;
            
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
                    SetForceDown(5f);
                    block.SetForceDown(5f);
                }
                else
                {
                    AddForceDown();
                    block.AddForceDown();
                }

                if (impulseDamage > intensityDamage)
                    block.TakeDamage(impulseDamage + BlockData.attack);
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
            
            float scaleX = _collider.transform.localScale.x * (BlockData.isFlip ? -1: 1);
            Vector2 flipScale = new Vector2(scaleX, _collider.transform.localScale.y);
            _collider.transform.localScale = flipScale;
        }

        private void ChangeBreakSprite()
        {
            float healthRatio = ((float)_currentHealth / BlockData.maxHealth) * 100;
            
            if (healthRatio > 75f)
                _spriteRenderer.sprite = BlockData.default_Sprite;
            else if (healthRatio > 50f)
                _spriteRenderer.sprite = BlockData.break_2_Sprite;
            else if (healthRatio > 25f)
                _spriteRenderer.sprite = BlockData.break_3_Sprite;
            else
                _spriteRenderer.sprite = BlockData.break_4_Sprite;
        }

        public void TakeDamage(int damage)
        {
            if(IsLock || CanDealDamage == false) return;
            
            OnHit?.Invoke();
            
            _currentHealth -= damage;
            
            ChangeBreakSprite();

            if (_currentHealth <= 0)
            {
                blockEventChannel.RaiseEvent(BlockEvent.CoundBlockEvent.Initialize(BlockData.destroyCount));
                DestroyBlock();
            }
        }

        public void Heal(int heal)
        {
            _currentHealth += heal;

            ChangeBreakSprite();
            
            if (BlockData.maxHealth < _currentHealth)
                _currentHealth = BlockData.maxHealth;
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
                                * Random.Range(BlockData.minDistance, BlockData.maxDistance);//방향
            direction += Vector2.up * Random.Range(BlockData.minHeight, BlockData.maxHeight);//높이
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
            
            _blockGuide.SetScale(BlockData.size);
        }
        
        [ContextMenu("DestroyBlock")]
        public void DestroyBlock()
        {
            if(_blockGuide != null)
                _blockGuide.SetGuiding(false);
            
            blockEventChannel.RaiseEvent(BlockEvent.DestroyBlockEvent.Initialize(this));
            
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }
}