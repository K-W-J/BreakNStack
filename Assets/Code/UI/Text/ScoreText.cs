using Code.Core;
using Code.Events;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Code.UI.Text
{
    public class ScoreText : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO uiEventChannel;
        [SerializeField] private TextMeshProUGUI scoreText;

        private int _scoreMultiply;
        private int _score;
        
        private bool _canShake = true;
        
        private void Awake()
        {
            uiEventChannel.AddListener<ScoreTextEvent>(HandleScoreText);
            uiEventChannel.AddListener<QuitGameEvent>(HandleResetScoreText);
            uiEventChannel.AddListener<ComboScoreTextEvent>(HandleComboScoreText);
        }

        private void HandleComboScoreText(ComboScoreTextEvent evt)
        {
            _scoreMultiply = evt.score <= 0 ? 1 : evt.score;
        }

        private void OnDestroy()
        {
            uiEventChannel.RemoveListener<ScoreTextEvent>(HandleScoreText);
            uiEventChannel.RemoveListener<QuitGameEvent>(HandleResetScoreText);
            uiEventChannel.RemoveListener<ComboScoreTextEvent>(HandleComboScoreText);
        }

        private void HandleResetScoreText(QuitGameEvent evt)
        {
            _score = 0;
            scoreText.SetText("");
        }

        private void HandleScoreText(ScoreTextEvent evt)
        {
            _score += evt.score * _scoreMultiply;
            
            if (_canShake)
            {
                _canShake = false;
                transform.DOShakeScale(0.1f).OnComplete(() => _canShake = true);
            }
            
            scoreText.SetText(_score.ToString());
            
            uiEventChannel.RaiseEvent(UIEvents.HighScoreTextEvent.Initialize(_score));
        }
    }
}