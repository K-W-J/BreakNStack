using System.Collections.Generic;
using Code.Agents;
using Code.Checkers;
using Code.Core;
using Code.Etc;
using Code.Events;
using UnityEngine;

namespace Code.Blocks
{
    public enum BlockState
    {
        None,
        Falling,
        Land,
        Lock,
    }
    
    public class Block : Agent, IDamageable, IPoolable
    {
        [field:SerializeField] public PoolItemSO PoolItem { get; private set; }
        public GameObject GameObject => gameObject;
        
        [SerializeField] private GameEventChannelSO blockEventChannel;
        [SerializeField] private GameEventChannelSO effectEventChannel;
        [SerializeField] private GameEventChannelSO uiEventChannel;
        [SerializeField] private PoolItemSO bumpEffectItem;
        [SerializeField] private PoolItemSO landEffectItem;
        [Space]
        [SerializeField] private float damageDelay;
        private float _currentDamageDelay;
        [SerializeField] private float stopMoveDelay;
        private float _currentStopMoveDelay;
        
        [field:Header("ResetBlock")]
        [field:SerializeField] public BlockSO BlockData { get; private set; }

        private List<Block> _touchBlocks;
        private IInitializeSpawn[] _initSpawns;

        public int CurrentHealth { get; private set; }
        
        private Rigidbody2D _rigidbody;
        
        private BoxOverlapChecker2D _boxChecker;
        
        private BlockColliderFixer _blockColliderFixer;
        private BlockRenderer _blockRenderer;
        private PoolManager _pool;

        private BlockState _blockState = BlockState.None;
        
        public float MovingVelocity => _rigidbody.linearVelocity.sqrMagnitude;
        
        private bool IsMove
        {
            get
            {
                if(stopMoveDelay > _currentStopMoveDelay) return true;
                
                bool isMove = _rigidbody.linearVelocity.sqrMagnitude > 0.000001f || 
                              Mathf.Abs(_rigidbody.angularVelocity) > 10f;
                
                if (isMove)
                    blockEventChannel.RaiseEvent(BlockEvents.BlockMoveEvent.Initialize(this));
                else
                    blockEventChannel.RaiseEvent(BlockEvents.BlockStopEvent.Initialize(this));
                
                return isMove;
            }
        }

        public bool IsLock => _blockState == BlockState.Lock;
        public bool IsLand => _blockState == BlockState.Land;

        private bool _isFirstLand;
        private bool _isFirstOnCollision;

        [ContextMenu("ResetBlock")]
        private void ResetBlock()
        {
            if(BlockData != null)
                SetUpPool(PoolManager.Instance);
        }

        protected override void Awake()
        {
            base.Awake();
            
            if(BlockData != null)
                SetUpPool(PoolManager.Instance);
        }
        
        public void SetUpPool(PoolManager pool)
        {
            _pool = pool;
            
            _rigidbody = GetComponentInChildren<Rigidbody2D>();
            _initSpawns = GetComponentsInChildren<IInitializeSpawn>();
            
            _boxChecker = GetModule<BoxOverlapChecker2D>();
            _blockRenderer = GetModule<BlockRenderer>();
            _blockColliderFixer = GetModule<BlockColliderFixer>();
 
            _touchBlocks = new List<Block>();

            if(BlockData != null)
                InitializeSpawn(BlockData);
        }
        
        public void InitializeSpawn(BlockSO blockSo)
        {
            Debug.Assert(blockSo != null, "not found BlockData.");
            
            BlockData = blockSo;
            
            _boxChecker.SetBoxSize(BlockData.size);

            foreach (var initSpawn in _initSpawns)
                initSpawn.InitializeSpawn();

            _rigidbody.mass = BlockData.weight;
            _rigidbody.simulated = false;
            
            _blockColliderFixer.UpdateColliderShape();
            
            CurrentHealth = BlockData.maxHealth;

            gameObject.name = $"{BlockData.blockType.ToString()}_{BlockData.blockName}_Block";
            _blockRenderer.SetFlip(BlockData.isFlip);
            
            SetFreezeAll(true);
        }

        private void Update()
        {
            if(IsLock || IsDead) return;

            if (damageDelay > _currentDamageDelay)
                _currentDamageDelay += Time.deltaTime;
            
            if (stopMoveDelay > _currentStopMoveDelay)
                _currentStopMoveDelay += Time.deltaTime;
            
            /*float limitLine = _camera.transform.position.y - (_camera.orthographicSize / 1.2f);
            
            if (_blockState == BlockState.Land && IsMove == false && limitLine > transform.position.y)
            {
                SetLockBlock(true);
            }*/
        }

        private void FixedUpdate()
        {
            if(IsLock || IsDead || IsMove) return;

            if (_blockState == BlockState.Falling)
            {
                SetBlockStateToLand();
            }
            else if (_blockState == BlockState.Land)
            {
                SetFreezeAll(true);

                if (_isFirstLand == false)
                {
                    blockEventChannel.RaiseEvent(BlockEvents.BlockLandEvent.Initialize(this));
                    uiEventChannel.RaiseEvent(UIEvents.ScoreTextEvent.Initialize(BlockData.stackPoint));
                    effectEventChannel.RaiseEvent(EffectEvents.PlayEffectEvent.Initialize(landEffectItem, transform.position));
                    _isFirstLand = true;
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(IsLock || IsDead) return;
            
            int impulseDamage = (int)collision.contacts[0].normalImpulse / 4;
            
            if (collision.gameObject.TryGetComponent<Block>(out var block))
            {
                if (impulseDamage > BlockData.intensityDamage && damageDelay < _currentDamageDelay)
                {
                    int totalDamage = 0;
                    
                    if(block.IsMove == false)
                        totalDamage = impulseDamage + block.BlockData.attack;
                    else
                        totalDamage = (impulseDamage + block.BlockData.attack) / 2;
                    
                    _currentDamageDelay = 0;
                    effectEventChannel.RaiseEvent(EffectEvents.PlayEffectEvent.Initialize(bumpEffectItem, collision.contacts[0].point));
                    
                    print(gameObject.name + " " + totalDamage);
                    
                    block.TakeDamage(totalDamage);
                }
                else
                {
                    if (_isFirstOnCollision == false)
                    {
                        block.TakeDamage(BlockData.attack * 2);
                        _isFirstOnCollision = true;
                    }
                }

                
                SetFreezeAll(false);
                OnFreezeTouchBlocks();

                blockEventChannel.RaiseEvent(BlockEvents.BlockTouchEvent.Initialize(this)) ;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if(IsLock || IsDead) return;

            OnFreezeTouchBlocks();
        }

        public void TakeDamage(int damage)
        {
            if(IsLock || IsDead || CanDealDamage == false) return;
            
            OnHitEvent?.Invoke();
            
            CurrentHealth -= damage;
            
            _blockRenderer.ChangeBreakSprite();

            if (CurrentHealth <= 0)
            {
                uiEventChannel.RaiseEvent(UIEvents.ScoreTextEvent.Initialize(BlockData.destroyPoint));
                PushBlock();
            }
        }

        public void Heal(int heal)
        {
            CurrentHealth += heal;

            _blockRenderer.ChangeBreakSprite();
            
            if (BlockData.maxHealth < CurrentHealth)
                CurrentHealth = BlockData.maxHealth;
        }

        private void SetFreezeAll(bool isFreeze)
        {
            if (isFreeze)
            {
                _rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
                _rigidbody.linearDamping = 0;
            }
            else
            {
                _currentStopMoveDelay = 0;
                _rigidbody.constraints = RigidbodyConstraints2D.None;
            }
        }
        
        private void OnFreezeTouchBlocks()
        {
            _touchBlocks.Clear();
            
            if (_boxChecker.TryGetOverlapData(_touchBlocks))
            {
                foreach (var adjacencyBlock in _touchBlocks)
                    adjacencyBlock.SetFreezeAll(false);
            }
        }

        private void SetLockBlock(bool isLock)
        {
            float intensity = isLock ? 0 : 1f;
            _blockRenderer.SetGrayScale(intensity);
            
            _blockState = isLock ? BlockState.Lock : BlockState.None;
            SetFreezeAll(isLock);

            if (isLock)
            {
                blockEventChannel.RemoveListener<BlockTouchEvent>(HandleTouchBlock);
            }
            else
            {
                blockEventChannel.AddListener<BlockTouchEvent>(HandleTouchBlock);
            }
        }

        [ContextMenu("DropBlock")]
        public void DropBlock()
        {
            SetFreezeAll(false);
            
            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.gravityScale = 3f;
            _rigidbody.simulated = true;
            _rigidbody.AddForce(Vector2.down * 1.5f, ForceMode2D.Force);

            _blockRenderer.SetAlpha(1);
            
            _blockState = BlockState.Falling;
        }
        
        private void SetBlockStateToLand()
        {
            _blockState = BlockState.Land;
        }
        
        [ContextMenu("PushBlocks")]
        public void PushBlock()
        {
            IsDead = true;
            gameObject.name = "Block[Pool]";

            OnFreezeTouchBlocks();
            
            blockEventChannel.RaiseEvent(BlockEvents.BlockPushEvent.Initialize(this));
            
            OnDeathEvent?.Invoke();
            _pool.Push(this);
        }
        
        public void ResetItem()
        {
            transform.rotation = Quaternion.identity;
            
            SetLockBlock(false);
            
            _blockRenderer.SetAlpha(0.8f);
            
            CurrentHealth = 0;
            IsDead = false;
            _isFirstOnCollision = false;
            _isFirstLand = false;
        }

        private void HandleTouchBlock(BlockTouchEvent evt)
        {
            if(IsDead || IsLock) return;
            
            OnFreezeTouchBlocks();
        }
    }
}