using Code.Core;
using Code.Events;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Text
{
    public class ComboScorePanel : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO uiEventChannel;
        [Space]
        [SerializeField] private GameObject fillBar;
        [SerializeField] private Image fillRect;
        [ColorUsage(true)] [SerializeField] private Color fillColor;
        [ColorUsage(true)] [SerializeField] private Color fillResetColor;
        [Space]
        [SerializeField] private TextMeshProUGUI comboScoreText;
        
        private float _comboTime;
        private bool _canShake = true;

        private void Awake()
        {
            uiEventChannel.AddListener<ComboScoreTextEvent>(HandleComboScoreText);
            uiEventChannel.AddListener<SetComboTimeEvent>(HandleSetComboTime);
            
            fillBar.SetActive(false);
            comboScoreText.SetText("");
        }

        private void OnDestroy()
        {
            uiEventChannel.RemoveListener<ComboScoreTextEvent>(HandleComboScoreText);
            uiEventChannel.RemoveListener<SetComboTimeEvent>(HandleSetComboTime);
        }
        
        private void HandleSetComboTime(SetComboTimeEvent evt)
        {
            if(fillRect.transform.localScale.x < 0) return;
                
            fillRect.transform.localScale = new Vector3(evt.time, 1, 1);
        }
  
        private void HandleComboScoreText(ComboScoreTextEvent evt)
        {
            if (evt.score > 0)
            {
                fillBar.SetActive(true);
                
                fillRect.DOColor(fillResetColor, 0.2f).OnComplete(() => fillRect.color = fillColor);
                comboScoreText.SetText("x" + evt.score);
            }
            else
            {
                fillBar.SetActive(false);
                
                comboScoreText.SetText("");
            }
            
            fillRect.transform.localScale = Vector3.one;
            
            if (_canShake)
            {
                _canShake = false;
                comboScoreText.transform.DOShakeScale(0.1f).OnComplete(() => _canShake = true);
            }
        }
    }
}