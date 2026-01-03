using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Effects
{
    public class ParticlePlayer : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particle;

        public bool IsPlaying => particle.isPlaying;
        private bool _isDestroy = false;
        
        private float _currentTime;
        
        private void Update()
        {
            if(_isDestroy == false || IsPlaying == false) return;
            
            if(particle.main.startLifetime.constant > _currentTime) 
                _currentTime += Time.deltaTime;
            else
                DestroyParticle();
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
        
        public void Play(bool isDestroy)
        {
            _isDestroy = isDestroy;
            particle.Play(true);
        }

        public void Play(Vector3 startPos)
        {   
             transform.position = startPos;
            particle.Play(true);
        }

        public void Stop() => particle.Stop(true);
        public void Pause() => particle.Pause(true);

        private void DestroyParticle()
        {
            Destroy(gameObject);
        }
    }
}