using System.Collections.Generic;
using Blade.Core;
using Code.Agents;
using Code.Core;
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
    
    public class Block : Agent, IDamageable, IPoolable
    {
        [field:SerializeField] public PoolItemSO PoolItem { get; private set; }
        public GameObject GameObject => gameObject;
        
        [SerializeField] private GameEventChannelSO blockEventChannel;

        [field:Header("ResetBlock")]
        [field:SerializeField] public BlockSO BlockData { get; private set; }

        private List<Block> _adjacencyBlocks;
        private IInitializeSpawn[] _initSpawns;

        public GameObject BlockCollider { get; private set; }
        public int CurrentHealth { get; private set; }
        
        private Rigidbody2D _rigidbody;
        private Camera _camera;
        
        private BlockRenderer _blockRenderer;
        private BlockGuide _blockGuide;
        private PoolManager _pool;

        private BlockState _blockState = BlockState.None;
        
        
        private bool IsMove => Mathf.Abs(_rigidbody.linearVelocity.y) > 0.0001f 
                               || Mathf.Abs(_rigidbody.linearVelocity.x) > 0.0001f; //Approximately은 판정이 너무 작음
        
        public bool IsLock => _blockState == BlockState.Lock;

        public bool IsFirstTimeStack { get; private set; }
        private bool _isFirstTimeGround;

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
            
            _camera = Camera.main;
            _rigidbody = GetComponentInChildren<Rigidbody2D>();
            _initSpawns = GetComponentsInChildren<IInitializeSpawn>();
            
            _blockRenderer = GetModule<BlockRenderer>();
 
            _adjacencyBlocks = new List<Block>();
            
            if(BlockData != null)
                InitializeBlockData(BlockData);
        }
        
        public void InitializeBlockData(BlockSO blockSo)
        {
            Debug.Assert(blockSo != null, "not found BlockData.");
            
            BlockData = blockSo;

            foreach (var initSpawn in _initSpawns)
                initSpawn.InitializeSpawn();

            _rigidbody.mass = BlockData.weight;
            
            BlockCollider = Instantiate(BlockData.colliderPrefab, transform);
            BlockCollider.transform.localScale = transform.localScale;
            
            CurrentHealth = BlockData.maxHealth;

            gameObject.name = $"{BlockData.blockType.ToString()}_{BlockData.blockName}_Block";
            _blockRenderer.SetFlip(BlockData.isFlip);
            
            FireBlock();
        }

        private void Update()
        {
            if(IsLock || IsDead) return;
            
            float limitLine = _camera.transform.position.y - (_camera.orthographicSize / 1.2f);
            
            if (_blockState == BlockState.Land && IsMove == false && limitLine > transform.position.y)
            {
                SetLockBlock(true);
            }
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

                if (IsFirstTimeStack == false)
                {
                    blockEventChannel.RaiseEvent(BlockEvent.CoundBlockEvent.Initialize(BlockData.stackCount));
                    IsFirstTimeStack = true;
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(IsLock || IsDead) return;
            
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

                if (impulseDamage > BlockData.intensityDamage)
                    block.TakeDamage(impulseDamage + BlockData.attack);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if(IsLock || IsDead) return;
            
            if (collision.gameObject.TryGetComponent<Block>(out var block))
                if (_adjacencyBlocks.Contains(block))
                {
                    _adjacencyBlocks.Remove(block);
                    SetFreezeAll(false);
                    AddForceDown();
                }
        }

        public void TakeDamage(int damage)
        {
            if(IsLock || IsDead || CanDealDamage == false) return;
            
            OnHit?.Invoke();
            
            CurrentHealth -= damage;
            
            _blockRenderer.ChangeBreakSprite();

            if (CurrentHealth <= 0)
            {
                blockEventChannel.RaiseEvent(BlockEvent.CoundBlockEvent.Initialize(BlockData.destroyCount));
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
            float intensity = isLock ? 0 : 1f;
            _blockRenderer.SetGrayScale(intensity);
            
            _blockState = isLock ? BlockState.Lock : BlockState.None;
            SetFreezeAll(isLock);
            
            if(isLock)
                blockEventChannel.RaiseEvent(BlockEvent.PushBlockEvent.Initialize(this));
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
        
        [ContextMenu("PushBlocks")]
        public void PushBlock()
        {
            if(_blockGuide != null)
                _blockGuide.SetGuiding(false);
            
            IsDead = true;
            gameObject.name = "Block[Pool]";
            _adjacencyBlocks.Clear();
            blockEventChannel.RaiseEvent(BlockEvent.PushBlockEvent.Initialize(this));
            blockEventChannel.RemoveListener<PushBlockEvent>(HandleTouchingBlockPush);
            
            OnDeath?.Invoke();
            _pool.Push(this);
        }
        
        public void ResetItem()
        {
            blockEventChannel.AddListener<PushBlockEvent>(HandleTouchingBlockPush);

            SetLockBlock(false);
            Destroy(BlockCollider);

            CurrentHealth = 0;
            IsDead = false;
            _isFirstTimeGround = false;
            IsFirstTimeStack = false;
        }

        private void HandleTouchingBlockPush(PushBlockEvent evt)
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
    }
}