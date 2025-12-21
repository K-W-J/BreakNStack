using System;
using Code.Checkers;
using UnityEngine;

namespace Code.Blocks
{
    public enum BlockState
    {
        None,
        Lock,
        Falling,
        Land,
    }
    public class Block : MonoBehaviour, IDamageable
    {
        private Rigidbody2D _rigidbody;
        private CastChecker2D _castChecker;
        
        private int _maxHealth;
        private int _currentHealth;
        
        public int Weight => _weight;
        private int _weight;

        public BlockState BlockState { get; private set; }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _castChecker = GetComponentInChildren<CastChecker2D>();
            
            //왜 iskni 사용 못
            _rigidbody.constraints = RigidbodyConstraints2D.FreezePosition;
            
            BlockState = BlockState.Lock;
            
            _maxHealth = 100;
            _currentHealth = _maxHealth;
            _weight = 20;
        }

        private void Update()
        {
            if (BlockState == BlockState.Falling && _castChecker.CastCheck())
            {
                SetBlockStateToLand();
            }
        }

        public void SetBlockStateToFalling()
        {
            _rigidbody.AddForce(Vector2.down * 1.5f, ForceMode2D.Force);
            _rigidbody.constraints = RigidbodyConstraints2D.None;
            BlockState = BlockState.Falling;
        }
        
        public void SetBlockStateToLand()
        {
            BlockState = BlockState.Land;
            
            GameObject[] blocks = _castChecker.GetCastData();
                
            foreach (GameObject block in blocks)
            {
                if (block != gameObject)
                {
                    IDamageable damageable = block.GetComponent<IDamageable>();
                        
                    if (damageable != null)
                        damageable.TakeDamage(_weight);
                }
            }
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;

            if (_currentHealth <= 0)
                DestroyBlock();
        }

        public void Heal(int heal)
        {
            _currentHealth += heal;
            
            if (_maxHealth < _currentHealth)
                _currentHealth = _maxHealth;
        }
        
        public void DestroyBlock()
        {
            Destroy(gameObject);
        }
    }
}