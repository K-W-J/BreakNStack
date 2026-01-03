
using Code.Effects;
using UnityEngine;

namespace Code.FeedBacks
{
    public class EffectFeedBack : FeedBack
    {
        [SerializeField] private ParticlePlayer particlePlayer;

        public override void CreateFeedback()
        {
            particlePlayer.Play(transform.position);
        }

        public override void StopFeedback()
        {
            particlePlayer.Stop();
        }

        private void OnDestroy()
        {
            if(particlePlayer != null && particlePlayer.IsPlaying == false)
                Destroy(particlePlayer.gameObject);
        }
    }
}