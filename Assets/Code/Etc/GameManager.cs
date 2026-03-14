using Code.Core;
using Code.Events;
using GondrLib.Dependencies;
using UnityEngine;

namespace Code.Etc
{
    enum GameState
    {
        Playing,
        Pause,
        End
    }
    
    [DefaultExecutionOrder(-10), Provide]
    public class GameManager : MonoBehaviour, IDependencyProvider
    {
        [SerializeField] private GameEventChannelSO uiEventChannel;
        
        private GameState _gameState = GameState.End;

        public bool IsPlayingGame => _gameState == GameState.Playing;
        public bool IsStopGame => _gameState == GameState.Pause;
        public bool IsEndGame => _gameState == GameState.End;
        
        private void Awake()
        {
            Application.targetFrameRate = 60;
            
            uiEventChannel.AddListener<PlayGameEvent>(HandlePlayGame);
            uiEventChannel.AddListener<QuitGameEvent>(HandleQuitGame);
            uiEventChannel.AddListener<PauseGameEvent>(HandlePauseGame);
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
            Time.timeScale = 1f;
        }
    }
}