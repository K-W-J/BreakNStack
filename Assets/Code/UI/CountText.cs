using Code.Core;
using Code.Events;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Code.UI
{
    public class CountText : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO blockEventChannel;
        [SerializeField] private TextMeshProUGUI text;
        
        private int _count;
        private bool _canShake = true;
        
        private void Awake()
        {
            blockEventChannel.AddListener<CountTextEvent>(SetCountText);
        }

        private void SetCountText(CountTextEvent evt)
        {
            _count += evt.count;
            
            if (_canShake)
            {
                _canShake = false;
                transform.DOShakeScale(0.1f).Complete(_canShake = true);
            }
            
            text.SetText(_count + "M");
        }

        private void OnDestroy()
        {
            blockEventChannel.RemoveListener<CountTextEvent>(SetCountText);
        }
    }
}