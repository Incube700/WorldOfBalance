using UnityEngine;
using UnityEngine.SceneManagement;

namespace WorldOfBalance.Managers
{
    /// <summary>
    /// Центральный менеджер игры, управляющий состояниями и игровым циклом.
    /// Отвечает за переходы между MainMenu, Playing, GameOver состояниями.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Game States")]
        [SerializeField] private GameState currentState = GameState.MainMenu;
        
        [Header("Scene Management")]
        [SerializeField] private string mainMenuScene = "MainMenu";
        [SerializeField] private string gameSceneName = "TankDuel";
        
        [Header("UI References")]
        [SerializeField] private GameObject mainMenuUI;
        [SerializeField] private GameObject gameplayUI;
        [SerializeField] private GameObject gameOverUI;
        
        // Singleton pattern
        public static GameManager Instance { get; private set; }
        
        // Game state properties
        public GameState CurrentState => currentState;
        
        // Events
        public System.Action<GameState> OnStateChanged;
        public System.Action<string> OnGameOver; // winner name
        
        void Awake()
        {
            // Singleton setup
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }
        
        void Start()
        {
            SetState(GameState.MainMenu);
        }
        
        /// <summary>
        /// Устанавливает новое состояние игры и выполняет соответствующие действия
        /// </summary>
        /// <param name="newState">Новое состояние</param>
        public void SetState(GameState newState)
        {
            if (currentState == newState) return;
            
            GameState previousState = currentState;
            currentState = newState;
            
            Debug.Log($"GameManager: State changed from {previousState} to {newState}");
            
            // Handle state transitions
            switch (newState)
            {
                case GameState.MainMenu:
                    HandleMainMenuState();
                    break;
                    
                case GameState.Playing:
                    HandlePlayingState();
                    break;
                    
                case GameState.GameOver:
                    HandleGameOverState();
                    break;
            }
            
            OnStateChanged?.Invoke(newState);
        }
        
        /// <summary>
        /// Начинает новую игру - переход в состояние Playing
        /// </summary>
        public void StartGame()
        {
            Debug.Log("GameManager: Starting new game");
            SetState(GameState.Playing);
        }
        
        /// <summary>
        /// Завершает игру с указанием победителя
        /// </summary>
        /// <param name="winner">Имя победителя</param>
        public void EndGame(string winner = "")
        {
            Debug.Log($"GameManager: Game ended. Winner: {winner}");
            OnGameOver?.Invoke(winner);
            SetState(GameState.GameOver);
        }
        
        /// <summary>
        /// Перезапускает текущий уровень
        /// </summary>
        public void RestartGame()
        {
            Debug.Log("GameManager: Restarting game");
            
            // Reload current scene
            string currentScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentScene);
            
            SetState(GameState.Playing);
        }
        
        /// <summary>
        /// Возвращает в главное меню
        /// </summary>
        public void ReturnToMainMenu()
        {
            Debug.Log("GameManager: Returning to main menu");
            
            if (!string.IsNullOrEmpty(mainMenuScene))
            {
                SceneManager.LoadScene(mainMenuScene);
            }
            
            SetState(GameState.MainMenu);
        }
        
        /// <summary>
        /// Выходит из игры
        /// </summary>
        public void QuitGame()
        {
            Debug.Log("GameManager: Quitting game");
            
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        
        void HandleMainMenuState()
        {
            // Show main menu UI
            SetUIState(mainMenuUI, true);
            SetUIState(gameplayUI, false);
            SetUIState(gameOverUI, false);
            
            // Pause game
            Time.timeScale = 1f;
        }
        
        void HandlePlayingState()
        {
            // Show gameplay UI
            SetUIState(mainMenuUI, false);
            SetUIState(gameplayUI, true);
            SetUIState(gameOverUI, false);
            
            // Resume game
            Time.timeScale = 1f;
            
            // Load game scene if needed
            if (SceneManager.GetActiveScene().name != gameSceneName && !string.IsNullOrEmpty(gameSceneName))
            {
                SceneManager.LoadScene(gameSceneName);
            }
        }
        
        void HandleGameOverState()
        {
            // Show game over UI
            SetUIState(mainMenuUI, false);
            SetUIState(gameplayUI, false);
            SetUIState(gameOverUI, true);
            
            // Pause game
            Time.timeScale = 0f;
        }
        
        void SetUIState(GameObject ui, bool active)
        {
            if (ui != null)
            {
                ui.SetActive(active);
            }
        }
        
        // Unity Events for UI buttons
        public void OnStartGameButton() => StartGame();
        public void OnRestartButton() => RestartGame();
        public void OnMainMenuButton() => ReturnToMainMenu();
        public void OnQuitButton() => QuitGame();
    }
    
    /// <summary>
    /// Возможные состояния игры
    /// </summary>
    public enum GameState
    {
        MainMenu,
        Playing, 
        GameOver
    }
}