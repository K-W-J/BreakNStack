using Code.Core;
using Code.Events;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Code.UI.Text
{
    public class HighScoreText : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO uiEventChannel;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private string addString;
        
        private int _score;
        private bool _canShake = true;
        
        private void Awake()
        {
            uiEventChannel.AddListener<HighScoreTextEvent>(HandleScoreText);
        }
        
        private void OnDestroy()
        {
            uiEventChannel.RemoveListener<HighScoreTextEvent>(HandleScoreText);
        }

        private void HandleResetScoreText(PlayGameEvent evt)
        {
            _score = 0;
            highScoreText.SetText(_score.ToString());
        }

        private void HandleScoreText(HighScoreTextEvent evt)
        {
            if (evt.highScore <= _score) return;

            _score = evt.highScore;
            
            if (_canShake)
            {
                _canShake = false;
                transform.DOShakeScale(0.1f).OnComplete(() => _canShake = true);
            }
            
            highScoreText.SetText(addString + _score);
        }
    }
}