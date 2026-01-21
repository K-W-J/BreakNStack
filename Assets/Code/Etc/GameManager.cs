using Code.Core;
using Code.Events;
using UnityEngine;

namespace Code.Etc
{
    [DefaultExecutionOrder(-100)]
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO uiEventChannel;
        
        public bool IsStartGame { get; private set; }
        
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<GameManager>();
                
                    if (_instance == null)
                    {
                        GameObject gameManager = new GameObject();
                        _instance = gameManager.AddComponent<GameManager>();
                        gameManager.name = typeof(GameManager) + "(Singleton)";
                    }
                }

                return _instance;
            }
        }
        
        private void Awake()
        {
            uiEventChannel.AddListener<PlayGameEvent>(HandleStartGame);
            
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            uiEventChannel.RemoveListener<PlayGameEvent>(HandleStartGame);
        }

        private void HandleStartGame(PlayGameEvent evt)
        {
            IsStartGame = true;
        }
    }
}