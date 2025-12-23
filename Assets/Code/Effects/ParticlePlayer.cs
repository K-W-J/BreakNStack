using System;
using UnityEngine;

namespace Code.Effects
{
    public class ParticlePlayer : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particle;

        private float _currentTime;
        
        private void Update()
        {
            if(particle.isPlaying == false) return;
            
            if(particle.main.startLifetime.constant > _currentTime) 
                _currentTime += Time.deltaTime;
            else
                DestroyParticle();
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

        private void DestroyParticle()
        {
            Destroy(gameObject);
        }
    }
}