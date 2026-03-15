using Ami.BroAudio;
using Code.Blocks;

namespace Code.Feedbacks
{
    public class SoundFeedback : Feedback, IFeedbackSettable
    {
        private SoundID _crashSoundId;
        
        public void SetFeedback(BlockSO blockSo)
        {
            _crashSoundId = blockSo.crashSound;
        }
        
        public override void CreateFeedback()
        {
            _crashSoundId.Play();
            //BroAudio.Play(_crashSoundId);
        }

        public override void StopFeedback()
        {
            BroAudio.Stop(_crashSoundId);
        }
    }
}