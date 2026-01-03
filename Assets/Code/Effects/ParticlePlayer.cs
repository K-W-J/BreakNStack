using System.Collections.Generic;
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
        
        private float _currentTime;
        
        public void SetUpPool(PoolManager pool)
        {
            _pool = pool;
        }
        
        public void ResetItem()
        {
            _currentTime = 0;
            Restart();
        }
        
        private void Update()
        {
            if(IsPlaying == false) return;
            
            if(particle.main.startLifetime.constant > _currentTime) 
                _currentTime += Time.deltaTime;
            else
                _pool.Push(this);
        }
        
        public void SetParticleSprites(List<Sprite> sprites)
        {
            foreach (var sprite in sprites)
                particle.textureSheetAnimation.AddSprite(sprite);
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