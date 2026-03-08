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
        
        private int _score;
        private bool _canShake = true;
        
        private void Awake()
        {
            uiEventChannel.AddListener<ScoreTextEvent>(HandleScoreText);
            uiEventChannel.AddListener<PlayGameEvent>(HandleResetScoreText);
        }
        
        private void OnDestroy()
        {
            uiEventChannel.RemoveListener<ScoreTextEvent>(HandleScoreText);
            uiEventChannel.RemoveListener<PlayGameEvent>(HandleResetScoreText);
        }

        private void HandleResetScoreText(PlayGameEvent evt)
        {
            _score = 0;
            scoreText.SetText(_score.ToString());
        }

        private void HandleScoreText(ScoreTextEvent evt)
        {
            _score += evt.score;
            
            if (_canShake)
            {
                _canShake = false;
                transform.DOShakeScale(0.1f).Complete(_canShake = true);
            }
            
            scoreText.SetText(_score + "m");
            
            uiEventChannel.RaiseEvent(UIEvents.HighScoreTextEvent.Initialize(_score));
        }
    }
}