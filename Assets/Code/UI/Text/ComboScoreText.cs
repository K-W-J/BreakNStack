using Code.Core;
using Code.Events;
using TMPro;
using UnityEngine;

namespace Code.UI.Text
{
    public class ComboScoreText : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO uiEventChannel;
        [SerializeField] private TextMeshProUGUI comboScoreText;

        private void Awake()
        {
            uiEventChannel.AddListener<ComboScoreTextEvent>(HandleComboScoreText);
        }

        private void OnDestroy()
        {
            uiEventChannel.RemoveListener<ComboScoreTextEvent>(HandleComboScoreText);
        }
  
        private void HandleComboScoreText(ComboScoreTextEvent evt)
        {
            comboScoreText.text = "x" + evt.score;
        }
    }
}