using Blade.Core;
using Code.Events;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Code.UI
{
    public class CountText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private GameEventChannelSO blockEventChannel;
        private int _count;
        private void Awake()
        {
            blockEventChannel.AddListener<CountBlockEvent>(SetCountText);
        }

        private void SetCountText(CountBlockEvent evt)
        {
            _count += evt.count;
            transform.DOShakeScale(0.1f);
            text.SetText(_count + "M");
        }

        private void OnDestroy()
        {
            blockEventChannel.RemoveListener<CountBlockEvent>(SetCountText);
        }
    }
}