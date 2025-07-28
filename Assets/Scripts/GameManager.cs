using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public float matchDuration = 300f; // 5 минут
    public int maxScore = 10;
    
    [Header("Players")]
    public PlayerController player1;
    public PlayerController player2;
    
    [Header("UI References")]
    public GameObject gameUI;
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    
    [Header("Prefabs")]
    public GameObject projectilePrefab;
    
    private float currentMatchTime;
    private bool isGamePaused = false;
    private bool isGameOver = false;
    
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
    
    void InitializeGame()
    {
        currentMatchTime = matchDuration;
        isGamePaused = false;
        isGameOver = false;
        
        // Находим игроков, если они не назначены
        if (player1 == null)
        {
            GameObject p1 = GameObject.FindGameObjectWithTag("Player");
            if (p1 != null) player1 = p1.GetComponent<PlayerController>();
        }
        
        if (player2 == null)
        {
            GameObject p2 = GameObject.FindGameObjectWithTag("Player2");
            if (p2 != null) player2 = p2.GetComponent<PlayerController>();
        }
        
        // Настраиваем UI
        if (gameUI != null) gameUI.SetActive(true);
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (gameOverMenu != null) gameOverMenu.SetActive(false);
        
        Debug.Log("Game initialized!");
    }
    
    void UpdateMatchTimer()
    {
        currentMatchTime -= Time.deltaTime;
        
        if (currentMatchTime <= 0)
        {
            EndMatch("Time's up!");
        }
    }
    
    void CheckGameOverConditions()
    {
        // Проверяем, жив ли хотя бы один игрок
        bool player1Alive = player1 != null && player1.gameObject.activeInHierarchy;
        bool player2Alive = player2 != null && player2.gameObject.activeInHierarchy;
        
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
    
    public void EndMatch(string reason)
    {
        isGameOver = true;
        Debug.Log($"Match ended: {reason}");
        
        if (gameOverMenu != null) gameOverMenu.SetActive(true);
        
        // Здесь можно добавить логику подсчета очков
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Создайте сцену главного меню
    }
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
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