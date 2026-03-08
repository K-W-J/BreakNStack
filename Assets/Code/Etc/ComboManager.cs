using Code.Core;
using Code.Events;
using UnityEngine;

namespace Code.Etc
{
    public class ComboManager : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO blockEventChannel;
        [SerializeField] private GameEventChannelSO uiEventChannel;
        [Space]
        [SerializeField] private float comboResetTime;
        [SerializeField] private int maxCombo;
        [SerializeField] private int[] combo;
        
        private float _comboTimer;
        private int _currentCombo;

        private void Awake()
        {
            blockEventChannel.AddListener<BlockLandEvent>(HandleIncreaseCombo);
        }

        private void OnDestroy()
        {
            blockEventChannel.RemoveListener<BlockLandEvent>(HandleIncreaseCombo);
        }

        private void HandleIncreaseCombo(BlockLandEvent _)
        {
            _comboTimer = comboResetTime;

            if (_currentCombo < maxCombo)
            {
                ++_currentCombo;
                uiEventChannel.RaiseEvent(UIEvents.ComboScoreTextEvent.Initialize(_currentCombo));
            }
        }

        private void Update()
        {
            if (_comboTimer < 0) return;
            
            _comboTimer -= Time.deltaTime;

            if (_comboTimer < 0)
            {
                _currentCombo = 0;
                uiEventChannel.RaiseEvent(UIEvents.ComboScoreTextEvent.Initialize(_currentCombo));
            }
        }
    }
}