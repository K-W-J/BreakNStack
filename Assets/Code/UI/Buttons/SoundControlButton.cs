using Ami.BroAudio;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Buttons
{
    public class SoundControlButton : MonoBehaviour
    {
        public enum SoundType
        {
            BGM,
            SFX,
        }
    
        public enum ToggleType
        {
            On,
            Off,
        }

        [SerializeField] private Sprite onSprite;
        [SerializeField] private Sprite offSprite;
        [SerializeField] private Image image;
        [Space]
        [SerializeField] private SoundType soundType;
        [SerializeField] private ToggleType toggleType;
        
        private bool IsOn => toggleType == ToggleType.On;

        public void ToggleButton()
        {
            toggleType = IsOn ? ToggleType.Off : ToggleType.On;
            image.sprite = IsOn ? onSprite : offSprite;
            float volume = IsOn ? 1 : 0;
            
            switch (soundType)
            {
                case SoundType.BGM :
                {
                    BroAudio.SetVolume(BroAudioType.Music, volume);
                    break;
                }
                case SoundType.SFX :
                {
                    BroAudio.SetVolume(BroAudioType.SFX, volume);
                    break;
                }
            }
        }
    }
}
