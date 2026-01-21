using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class PopWindow : MonoBehaviour
    {
        [SerializeField] private Button offWindowButton;

        private void Awake()
        {
            offWindowButton.onClick.AddListener(HandleOffWindow);
        }

        private void OnDestroy()
        {
            offWindowButton.onClick.RemoveListener(HandleOffWindow);
        }

        private void HandleOffWindow()
        {
            gameObject.SetActive(false);
        }
    }
}