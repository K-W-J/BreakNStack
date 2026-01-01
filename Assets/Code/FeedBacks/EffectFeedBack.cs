
using Code.Effects;
using UnityEngine;

namespace Code.FeedBacks
{
    public class EffectFeedBack : FeedBack
    {
        private ParticlePlayer _particlePlayer;

        public override void CreateFeedback()
        {
            _particlePlayer.Play(transform.position);
        }

        public override void StopFeedback()
        {
            _particlePlayer.Stop();
        }

        private void OnDestroy()
        {
            if(_particlePlayer != null && _particlePlayer.IsPlaying == false)
                Destroy(_particlePlayer.gameObject);
        }
    }
}