using Code.Core;
using Code.Events;
using UnityEngine;

namespace Code.Etc
{
    enum GameState
    {
        Playing,
        Pause,
        End
    }
    
    [DefaultExecutionOrder(-100)]
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO uiEventChannel;
        
        private GameState _gameState;

        public bool IsPlayingGame => _gameState == GameState.Playing;
        public bool IsStopGame => _gameState == GameState.Pause;
        public bool IsEndGame => _gameState == GameState.End;
        
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
            uiEventChannel.AddListener<PlayGameEvent>(HandlePlayGame);
            uiEventChannel.AddListener<QuitGameEvent>(HandleQuitGame);
            uiEventChannel.AddListener<PauseGameEvent>(HandlePauseGame);
            
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
            uiEventChannel.RemoveListener<PlayGameEvent>(HandlePlayGame);
            uiEventChannel.RemoveListener<QuitGameEvent>(HandleQuitGame);
            uiEventChannel.RemoveListener<PauseGameEvent>(HandlePauseGame);
        }

        private void HandlePauseGame(PauseGameEvent evt)
        {
            _gameState = GameState.Pause;
            Time.timeScale = 0f;
        }

        private void HandlePlayGame(PlayGameEvent evt)
        {
            _gameState = GameState.Playing;
            Time.timeScale = 1f;
        }
        
        private void HandleQuitGame(QuitGameEvent evt)
        {
            _gameState = GameState.End;
        }
    }
}