using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Простой менеджер игры для WorldOfBalance - основной игровой цикл
/// От стартового экрана до конца боя
/// </summary>
public class SimpleGameManager : MonoBehaviour
{
    [Header("Game States")]
    [SerializeField] private GameState currentState = GameState.MainMenu;
    
    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel;
    
    [Header("Game Settings")]
    [SerializeField] private string gameSceneName = "TankDuel";
    
    // Singleton
    public static SimpleGameManager Instance { get; private set; }
    
    // Game state
    public GameState CurrentState => currentState;
    
    // Game data
    private string winnerName = "";
    private float gameTime = 0f;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        SetState(GameState.MainMenu);
    }
    
    void Update()
    {
        if (currentState == GameState.Playing)
        {
            gameTime += Time.deltaTime;
        }
    }
    
    /// <summary>
    /// Изменяет состояние игры
    /// </summary>
    public void SetState(GameState newState)
    {
        if (currentState == newState) return;
        
        Debug.Log($"SimpleGameManager: {currentState} -> {newState}");
        currentState = newState;
        
        switch (newState)
        {
            case GameState.MainMenu:
                ShowMainMenu();
                break;
                
            case GameState.Playing:
                StartGame();
                break;
                
            case GameState.GameOver:
                ShowGameOver();
                break;
                
            case GameState.Victory:
                ShowVictory();
                break;
        }
    }
    
    void ShowMainMenu()
    {
        Time.timeScale = 1f;
        SetPanelActive(mainMenuPanel, true);
        SetPanelActive(gameplayPanel, false);
        SetPanelActive(gameOverPanel, false);
        SetPanelActive(winPanel, false);
    }
    
    void StartGame()
    {
        Time.timeScale = 1f;
        gameTime = 0f;
        winnerName = "";
        
        SetPanelActive(mainMenuPanel, false);
        SetPanelActive(gameplayPanel, true);
        SetPanelActive(gameOverPanel, false);
        SetPanelActive(winPanel, false);
        
        // Load game scene if needed
        if (SceneManager.GetActiveScene().name != gameSceneName)
        {
            SceneManager.LoadScene(gameSceneName);
        }
    }
    
    void ShowGameOver()
    {
        Time.timeScale = 0f;
        SetPanelActive(mainMenuPanel, false);
        SetPanelActive(gameplayPanel, false);
        SetPanelActive(gameOverPanel, true);
        SetPanelActive(winPanel, false);
    }
    
    void ShowVictory()
    {
        Time.timeScale = 0f;
        SetPanelActive(mainMenuPanel, false);
        SetPanelActive(gameplayPanel, false);
        SetPanelActive(gameOverPanel, false);
        SetPanelActive(winPanel, true);
    }
    
    void SetPanelActive(GameObject panel, bool active)
    {
        if (panel != null)
        {
            panel.SetActive(active);
        }
    }
    
    // Public methods for UI buttons
    public void OnStartGameButton()
    {
        Debug.Log("SimpleGameManager: Start Game pressed");
        SetState(GameState.Playing);
    }
    
    public void OnRestartButton()
    {
        Debug.Log("SimpleGameManager: Restart pressed");
        SetState(GameState.Playing);
    }
    
    public void OnMainMenuButton()
    {
        Debug.Log("SimpleGameManager: Main Menu pressed");
        SetState(GameState.MainMenu);
    }
    
    public void OnQuitButton()
    {
        Debug.Log("SimpleGameManager: Quit pressed");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    /// <summary>
    /// Вызывается когда танк уничтожен
    /// </summary>
    public void OnTankDestroyed(string tankName)
    {
        Debug.Log($"SimpleGameManager: Tank {tankName} destroyed!");
        
        if (tankName.ToLower().Contains("player"))
        {
            // Player died - game over
            winnerName = "Enemy";
            SetState(GameState.GameOver);
        }
        else
        {
            // Enemy died - victory
            winnerName = "Player";
            SetState(GameState.Victory);
        }
    }
    
    /// <summary>
    /// Получить время игры
    /// </summary>
    public float GetGameTime()
    {
        return gameTime;
    }
    
    /// <summary>
    /// Получить имя победителя
    /// </summary>
    public string GetWinner()
    {
        return winnerName;
    }
}

/// <summary>
/// Состояния игры
/// </summary>
public enum GameState
{
    MainMenu,   // Стартовый экран
    Playing,    // Бой
    GameOver,   // Поражение
    Victory     // Победа
}