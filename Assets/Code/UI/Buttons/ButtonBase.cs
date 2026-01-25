using Code.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Buttons
{
    public class ButtonBase : MonoBehaviour
    {
        [SerializeField] protected GameEventChannelSO uiEventChannel; 
        protected Button Button;
        
        protected virtual void Awake()
        {
            Button = GetComponent<Button>();
        }
        
        protected virtual void OnDestroy()
        {
        }
    }
}