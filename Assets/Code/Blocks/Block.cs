using System;
using UnityEngine;

namespace Code.Blocks
{
    public enum BlockState
    {
        None,
        Lock,
        Fall,
    }
    public class Block : MonoBehaviour, IDamageable
    {
        private Rigidbody2D _rigidbody;
        
        private int _maxHealth;
        private int _currentHealth;
        
        public int Weight => _weight;
        private int _weight;

        public BlockState BlockState { get; private set; }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _rigidbody.simulated = false;
            
            _maxHealth = 100;
            _weight = 20;
        }
        
        public void SetFall()
        {
            BlockState = BlockState.Fall;
            _rigidbody.simulated = true;
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