using Ami.BroAudio;
using Code.Blocks;
using UnityEngine;

namespace Code.Feedbacks
{
    public class SoundFeedback : Feedback, IFeedbackSettable
    {
        [SerializeField] private SoundID crashSoundId;
        
        public void SetFeedback(BlockSO blockSo)
        {
            crashSoundId = blockSo.crashSound;
        }
        
        public override void CreateFeedback()
        {
            crashSoundId.Play();
            //BroAudio.Play(_crashSoundId);
        }

        public override void StopFeedback()
        {
            BroAudio.Stop(crashSoundId);
        }
    }
}