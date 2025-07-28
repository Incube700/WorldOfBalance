using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Управление игровым процессом и состоянием игры
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public float matchDuration = 300f; // 5 минут
    public int maxScore = 10;
    
    [Header("UI References")]
    public GameObject gameUI;
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    
    [Header("Managers")]
    public SpawnManager spawnManager;
    
    private float currentMatchTime;
    private bool isGamePaused = false;
    private bool isGameOver = false;
    private GameObject player1;
    private GameObject player2;
    
    // Синглтон
    public static GameManager Instance { get; private set; }
    
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
        InitializeGame();
    }
    
    void Update()
    {
        if (!isGamePaused && !isGameOver)
        {
            UpdateMatchTimer();
            CheckGameOverConditions();
        }
        
        HandleInput();
    }
    
    /// <summary>
    /// Инициализирует игру
    /// </summary>
    void InitializeGame()
    {
        currentMatchTime = matchDuration;
        isGamePaused = false;
        isGameOver = false;
        
        // Находим SpawnManager, если он не назначен
        if (spawnManager == null)
        {
            spawnManager = FindObjectOfType<SpawnManager>();
        }
        
        // Настраиваем UI
        if (gameUI != null) gameUI.SetActive(true);
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (gameOverMenu != null) gameOverMenu.SetActive(false);
        
        Debug.Log("Game initialized!");
    }
    
    /// <summary>
    /// Обновляет таймер матча
    /// </summary>
    void UpdateMatchTimer()
    {
        currentMatchTime -= Time.deltaTime;
        
        if (currentMatchTime <= 0)
        {
            EndMatch("Time's up!");
        }
    }
    
    /// <summary>
    /// Проверяет условия окончания игры
    /// </summary>
    void CheckGameOverConditions()
    {
        // Проверяем, жив ли хотя бы один игрок
        bool player1Alive = IsPlayerAlive("Player");
        bool player2Alive = IsPlayerAlive("Player2");
        
        if (!player1Alive && !player2Alive)
        {
            EndMatch("Both players destroyed!");
        }
        else if (!player1Alive)
        {
            EndMatch("Player 2 wins!");
        }
        else if (!player2Alive)
        {
            EndMatch("Player 1 wins!");
        }
    }
    
    /// <summary>
    /// Проверяет, жив ли игрок
    /// </summary>
    /// <param name="playerTag">Тег игрока</param>
    /// <returns>true если игрок жив</returns>
    bool IsPlayerAlive(string playerTag)
    {
        if (spawnManager != null)
        {
            return spawnManager.IsPlayerAlive(playerTag);
        }
        
        // Fallback: ищем игрока в сцене
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            DamageSystem damageSystem = player.GetComponent<DamageSystem>();
            return damageSystem != null && damageSystem.IsAlive();
        }
        
        return false;
    }
    
    /// <summary>
    /// Обрабатывает ввод пользователя
    /// </summary>
    void HandleInput()
    {
        // Пауза
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        
        // Рестарт
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }
    
    /// <summary>
    /// Переключает паузу
    /// </summary>
    public void TogglePause()
    {
        isGamePaused = !isGamePaused;
        
        if (isGamePaused)
        {
            Time.timeScale = 0f;
            if (pauseMenu != null) pauseMenu.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            if (pauseMenu != null) pauseMenu.SetActive(false);
        }
    }
    
    /// <summary>
    /// Завершает матч
    /// </summary>
    /// <param name="reason">Причина окончания матча</param>
    public void EndMatch(string reason)
    {
        isGameOver = true;
        Debug.Log($"Match ended: {reason}");
        
        if (gameOverMenu != null) gameOverMenu.SetActive(true);
    }
    
    /// <summary>
    /// Перезапускает игру
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    /// <summary>
    /// Возвращается в главное меню
    /// </summary>
    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    
    /// <summary>
    /// Выходит из игры
    /// </summary>
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    /// <summary>
    /// Вызывается при смерти игрока
    /// </summary>
    /// <param name="deadPlayer">Объект мертвого игрока</param>
    public void OnPlayerDeath(GameObject deadPlayer)
    {
        Debug.Log($"Player {deadPlayer.name} died!");
        
        // Респавним игрока через некоторое время
        if (spawnManager != null)
        {
            string playerTag = deadPlayer.tag;
            spawnManager.RespawnPlayerWithDelay(playerTag, 3f);
        }
    }
    
    /// <summary>
    /// Вызывается при создании игроков
    /// </summary>
    /// <param name="player1">Игрок 1</param>
    /// <param name="player2">Игрок 2</param>
    public void OnPlayersSpawned(GameObject player1, GameObject player2)
    {
        this.player1 = player1;
        this.player2 = player2;
        
        Debug.Log("Players spawned successfully!");
    }
    
    /// <summary>
    /// Вызывается при респауне игрока
    /// </summary>
    /// <param name="respawnedPlayer">Респавнутый игрок</param>
    public void OnPlayerRespawned(GameObject respawnedPlayer)
    {
        Debug.Log($"Player {respawnedPlayer.name} respawned!");
    }
    
    // Геттеры для UI
    public float GetMatchTime()
    {
        return currentMatchTime;
    }
    
    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(currentMatchTime / 60f);
        int seconds = Mathf.FloorToInt(currentMatchTime % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    
    public bool IsGamePaused()
    {
        return isGamePaused;
    }
    
    public bool IsGameOver()
    {
        return isGameOver;
    }
} 