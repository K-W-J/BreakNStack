using Code.Core;
using Code.Defines;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.PopupWindows
{
    public class PopupWindow : MonoBehaviour
    {
        [SerializeField] protected GameEventChannelSO uiEventChannel;
        
        [field: SerializeField] public WindowType WindowType { get; private set; }
        
        [SerializeField] protected Button offWindowButton;

        protected virtual void Awake()
        {
            offWindowButton.onClick.AddListener(HandleOffWindow);
        }

        protected virtual void OnDestroy()
        {
            offWindowButton.onClick.RemoveListener(HandleOffWindow);
        }

        protected virtual void HandleOffWindow()
        {
            gameObject.SetActive(false);
        }
    }
}