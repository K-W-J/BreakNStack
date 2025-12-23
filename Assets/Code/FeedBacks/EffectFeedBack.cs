
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
            GameObject particleObject = Instantiate(particlePlayer);
            _particlePlayer = particleObject.GetComponent<ParticlePlayer>();
            _particlePlayer.transform.SetParent(null);
        }

        public override void CreateFeedback()
        {
            _particlePlayer.Play(transform.position);
        }

        public override void StopFeedback()
        {
            _particlePlayer.Stop();
        }
    }
}