using Code.Core;
using UnityEngine;

namespace Code.Effects
{
    public class ParticlePlayer : MonoBehaviour, IPoolable
    {
        [field: SerializeField] public PoolItemSO PoolItem { get; private set; }
        [SerializeField] private ParticleSystem particle;
        
        public GameObject GameObject => gameObject;
        private PoolManager _pool;
        
        public bool IsPlaying => particle.isPlaying;
        
        public void SetUpPool(PoolManager pool)
        {
            _pool = pool;
        }
        
        public void ResetItem()
        {
            Restart();
        }
        
        private void Update()
        {
            if (IsPlaying == false)
                _pool.Push(this);
        }
        
        public void Restart()
        {
            Stop();
            Play();
        }
        
        public void Play() => particle.Play(true);
        public void Play(Vector3 startPos)
        {   
             transform.position = startPos;
            particle.Play(true);
        }

        public void Stop() => particle.Stop(true);
        public void Pause() => particle.Pause(true);
    }
}