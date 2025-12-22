using System;
using Code.Checkers;
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
    
    public class Block : MonoBehaviour, IDamageable
    {
        private static readonly int Contrast = Shader.PropertyToID("_Contrast");
        
        private Rigidbody2D _rigidbody;
        private Material _grayMat;
        private Camera _camera;
        
        private CastChecker2D _castChecker;
        
        [SerializeField] private int maxHealth;
        [SerializeField] private int currentHealth;
        [SerializeField] private int weight;
        
        [SerializeField] private int maxHeight;
        [SerializeField] private int minHeight;
        [SerializeField] private int maxDistance;
        [SerializeField] private int minDistance;

        private BlockState _blockState;
        
        private bool IsMoveY => _rigidbody.linearVelocity.y == 0f;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _castChecker = GetComponentInChildren<CastChecker2D>();
            _grayMat = GetComponentInChildren<SpriteRenderer>().material;
            
            _camera = Camera.main;

            _blockState = BlockState.None;

            currentHealth = maxHealth;
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
            if (_blockState == BlockState.Falling //&& _castChecker.CastCheck()
                && IsMoveY)
            {
                SetBlockStateToLand();
            }
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
            
            Vector2 direction = (transform.position.x > 0 ? Vector2.left : Vector2.right) * Random.Range(minDistance, maxDistance);;
            direction += Vector2.up * Random.Range(minHeight, maxHeight);
            
            _rigidbody.gravityScale = 0.5f;
            _rigidbody.AddForce(direction, ForceMode2D.Impulse);
        }

        public void SetLockBlock(bool isLock)
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
            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.gravityScale = 3f;
            
            _rigidbody.AddForce(Vector2.down * 1.5f, ForceMode2D.Force);
            _blockState = BlockState.Falling;
            SetFreezePosition(false);
        }
        
        public void SetBlockStateToLand()
        {
            _blockState = BlockState.Land;
            
            GameObject[] blocks = _castChecker.GetCastData();
                
            foreach (GameObject block in blocks)
            {
                if (block != gameObject)
                {
                    IDamageable damageable = block.GetComponent<IDamageable>();
                        
                    if (damageable != null)
                        damageable.TakeDamage(weight);
                }
            }
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;

            if (currentHealth <= 0)
                DestroyBlock();
        }

        public void Heal(int heal)
        {
            currentHealth += heal;
            
            if (maxHealth < currentHealth)
                currentHealth = maxHealth;
        }

        private void DestroyBlock()
        {
            Destroy(gameObject);
        }
    }
}