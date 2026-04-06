using System.Collections.Generic;
using System.Linq;
using Code.Core;
using Code.Defines;
using Code.Events;
using UnityEngine;

namespace Code.UI.PopupWindows
{
    public class WindowManager : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO uiEventChannel;
        [SerializeField] private bool isActiveWindow;
        
        private Dictionary<WindowType, PopupWindow> _popWindowDict;

        private void Awake()
        {
            uiEventChannel.AddListener<OpenWindowEvent>(HandleOpenWindow);
            uiEventChannel.AddListener<CloseWindowEvent>(HandleCloseWindow);

            _popWindowDict = GetComponentsInChildren<PopupWindow>(true).ToDictionary(window => window.WindowType);

            if (isActiveWindow)
            {
                foreach (var popWindow in _popWindowDict)
                    popWindow.Value.gameObject.SetActive(false);
            }
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