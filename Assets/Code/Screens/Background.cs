using Code.Core;
using Code.Events;
using UnityEngine;

namespace Code.Screens
{
    public class Background : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO uiEventChannel;
        [Space]
        [SerializeField] private SpriteRenderer background1;
        [SerializeField] private SpriteRenderer background2;
        [SerializeField] private int forwardSortingOrder;
        [SerializeField] private int backSortingOrder;
        [Space]
        [SerializeField] private float downwardOffset;
        
        private float _originBackground1;
        private float _originBackground2;
        private bool _isBackground1Lower;

        private void Awake()
        {
            uiEventChannel.AddListener<QuitGameEvent>(HandleQuitGame);
            
            _originBackground1 = background1.transform.position.y;
            _originBackground2 = background2.transform.position.y;
        }

        private void HandleQuitGame(QuitGameEvent evt)
        {
            _isBackground1Lower = false;
            
            background1.transform.position = new Vector2(0, _originBackground1);
            background2.transform.position = new Vector2(0, _originBackground2);
            
            background2.sortingOrder = forwardSortingOrder;
            background1.sortingOrder = backSortingOrder;
        }

        private void OnDestroy()
        {
            uiEventChannel.RemoveListener<QuitGameEvent>(HandleQuitGame);
        }

        private void Update()
        {
            if (background1.transform.position.y < ScreenInfo.ScreenPosition.y && !_isBackground1Lower)
            {
                _isBackground1Lower = true;
                background2.transform.position += Vector3.up * (background1.bounds.size.y * 2 - downwardOffset);
                
                background1.sortingOrder = forwardSortingOrder;
                background2.sortingOrder = backSortingOrder;
            }
            else if (background2.transform.position.y < ScreenInfo.ScreenPosition.y && _isBackground1Lower)
            {
                _isBackground1Lower = false;
                background1.transform.position += Vector3.up * (background2.bounds.size.y * 2 - downwardOffset);
                
                background2.sortingOrder = forwardSortingOrder;
                background1.sortingOrder = backSortingOrder;
            }
        }
    }
}