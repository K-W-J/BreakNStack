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
        [SerializeField] private float minAdjacencyBlockMove;
        
        [field:Header("ResetBlock")]
        [field:SerializeField] public BlockSO BlockData { get; private set; }

        private List<Block> _adjacencyBlocks;
        private IInitializeSpawn[] _initSpawns;

        public GameObject BlockCollider { get; private set; }
        public int CurrentHealth { get; private set; }
        
        private Rigidbody2D _rigidbody;
        
        private BoxOverlapChecker2D _boxChecker2D;
        private BlockRenderer _blockRenderer;
        private PoolManager _pool;

        private BlockState _blockState = BlockState.None;
        
        private bool IsMove
        {
            get
            {
                if(stopMoveDelay > _currentStopMoveDelay) return true;
                
                bool isMove = _rigidbody.linearVelocity.sqrMagnitude > 0.0000001f || 
                              Mathf.Abs(_rigidbody.angularVelocity) > 10f;
                
                bool isAdjacencyBlockMove = _rigidbody.linearVelocity.sqrMagnitude > 0.001f || 
                                            Mathf.Abs(_rigidbody.angularVelocity) > 10f;
                //Approximately은 판정이 너무 타이트함
                
                if(isAdjacencyBlockMove)
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
            Debug.Assert(BlockData != null, "not found BlockData.");
            
            gameObject.name = $"{BlockData.blockType.ToString()}_{BlockData.blockName}_Block";      
            
            BlockRenderer blockRenderer = GetComponentInChildren<BlockRenderer>();
            blockRenderer.InitializeSpawn();

            if(BlockCollider != null)
                DestroyImmediate(BlockCollider);
            
            BlockCollider = Instantiate(BlockData.colliderPrefab, transform);
            BlockCollider.transform.localScale = Vector3.one;
            
            float scaleX = BlockCollider.transform.localScale.x * (BlockData.isFlip ? -1: 1);
            Vector2 flipScale = new Vector2(scaleX, BlockCollider.transform.localScale.y);
            BlockCollider.transform.localScale = flipScale;
        }
        
        public void SetUpPool(PoolManager pool)
        {
            _pool = pool;
            
            _rigidbody = GetComponentInChildren<Rigidbody2D>();
            _initSpawns = GetComponentsInChildren<IInitializeSpawn>();
            
            _boxChecker2D = GetModule<BoxOverlapChecker2D>();
            _blockRenderer = GetModule<BlockRenderer>();
 
            _adjacencyBlocks = new List<Block>();

            if(BlockData != null)
                InitializeSpawn(BlockData);
        }
        
        public void InitializeSpawn(BlockSO blockSo)
        {
            Debug.Assert(blockSo != null, "not found BlockData.");
            
            BlockData = blockSo;
            
            _boxChecker2D.SetBoxSize(BlockData.size);

            foreach (var initSpawn in _initSpawns)
                initSpawn.InitializeSpawn();

            _rigidbody.mass = BlockData.weight;
            _rigidbody.simulated = false;
            
            BlockCollider = Instantiate(BlockData.colliderPrefab, transform);
            BlockCollider.transform.localScale = transform.localScale;
            
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
                    uiEventChannel.RaiseEvent(UIEvents.ScoreTextEvent.Initialize(BlockData.stackCount));
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
                OnFreezeAdjacencyBlocks();
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if(IsLock || IsDead) return;

            OnFreezeAdjacencyBlocks();
        }

        public void TakeDamage(int damage)
        {
            if(IsLock || IsDead || CanDealDamage == false) return;
            
            OnHitEvent?.Invoke();
            
            CurrentHealth -= damage;
            
            _blockRenderer.ChangeBreakSprite();

            if (CurrentHealth <= 0)
            {
                uiEventChannel.RaiseEvent(UIEvents.ScoreTextEvent.Initialize(BlockData.destroyCount));
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
        
        private void OnFreezeAdjacencyBlocks()
        {
            _adjacencyBlocks.Clear();
            
            if (_boxChecker2D.TryGetOverlapData(_adjacencyBlocks))
            {
                foreach (var adjacencyBlock in _adjacencyBlocks)
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
                blockEventChannel.RemoveListener<BlockMoveEvent>(HandleTouchingBlockMove);
            }
            else
            {
                blockEventChannel.AddListener<BlockMoveEvent>(HandleTouchingBlockMove);
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

            OnFreezeAdjacencyBlocks();
            
            blockEventChannel.RaiseEvent(BlockEvents.BlockPushEvent.Initialize(this));
            blockEventChannel.RemoveListener<BlockMoveEvent>(HandleTouchingBlockMove);
            
            OnDeathEvent?.Invoke();
            _pool.Push(this);
        }
        
        public void ResetItem()
        {
            transform.rotation = Quaternion.identity;
            
            SetLockBlock(false);
            Destroy(BlockCollider);
            
            _blockRenderer.SetAlpha(0.8f);
            
            CurrentHealth = 0;
            IsDead = false;
            _isFirstOnCollision = false;
            _isFirstLand = false;
        }

        private void HandleTouchingBlockMove(BlockMoveEvent evt)
        {
            if(IsLock) return;

            //OnFreezeAdjacencyBlocks();
        }
    }
}