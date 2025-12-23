
using System;
using Code.Effects;
using UnityEngine;

namespace Code.FeedBacks
{
    public class EffectFeedBack : FeedBack
    {
        [SerializeField] private GameObject particlePlayer;
        
        private ParticlePlayer _particlePlayer;

        private void Awake()
        {
            _particlePlayer = particlePlayer.GetComponent<ParticlePlayer>();
        }

        public override void CreateFeedback()
        {
            _particlePlayer.Play();
        }

        public override void StopFeedback()
        {
            _particlePlayer.Stop();
        }
    }
}