using System.Collections.Generic;
using System.Linq;
using Code.Core;
using Code.Define;
using Code.Events;
using UnityEngine;

namespace Code.UI.PopWindows
{
    public class WindowManager : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO uiEventChannel;
        private Dictionary<WindowType, PopWindow> _popWindowDict;

        private void Awake()
        {
            uiEventChannel.AddListener<OpenWindowEvent>(HandleOpenWindow);
            uiEventChannel.AddListener<CloseWindowEvent>(HandleCloseWindow);

            _popWindowDict = GetComponentsInChildren<PopWindow>(true).ToDictionary(window => window.WindowType);
        }

        private void OnDestroy()
        {
            uiEventChannel.RemoveListener<OpenWindowEvent>(HandleOpenWindow);
            uiEventChannel.RemoveListener<CloseWindowEvent>(HandleCloseWindow);
        }

        private void HandleOpenWindow(OpenWindowEvent evt)
        {
            Debug.Assert(_popWindowDict.ContainsKey(evt.windowType), "popWindowDict not found key");
            
            _popWindowDict[evt.windowType].gameObject.SetActive(true);
        }
        
        private void HandleCloseWindow(CloseWindowEvent evt)
        {
            _popWindowDict[evt.windowType].gameObject.SetActive(false);
        }
    }
}