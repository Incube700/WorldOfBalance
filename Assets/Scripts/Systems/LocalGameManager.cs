using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalGameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private bool isGamePaused = false;
    
    [Header("Player References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private HealthSystem playerHealth;
    
    public static LocalGameManager Instance { get; private set; }
    
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
        if (player == null)
            player = FindObjectOfType<PlayerController>();
            
        if (playerHealth == null && player != null)
            playerHealth = player.GetComponent<HealthSystem>();
            
        // Отключаем игровые компоненты до старта игры
        DisableGameComponents();
        
        // Останавливаем время
        Time.timeScale = 0f;
        
        Debug.Log("Игра готова! Нажмите R для запуска!");
    }
    
    void Update()
    {
        if (Time.timeScale > 0) // Только если игра не на паузе
        {
            HandleInput();
            CheckGameState();
        }
    }
    
    void DisableGameComponents()
    {
        // Отключаем всех игроков до старта игры
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        foreach (PlayerController player in players)
        {
            if (player != null)
                player.enabled = false;
        }
        
        // Отключаем ИИ врагов
        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
        foreach (EnemyAI enemy in enemies)
        {
            if (enemy != null)
                enemy.enabled = false;
        }
        
        Debug.Log("Игровые компоненты отключены до старта!");
    }
    
    public void EnableGameComponents()
    {
        // Включаем всех игроков
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        foreach (PlayerController player in players)
        {
            if (player != null)
                player.enabled = true;
        }
        
        // Включаем ИИ врагов
        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
        foreach (EnemyAI enemy in enemies)
        {
            if (enemy != null)
                enemy.enabled = true;
        }
        
        Debug.Log("Игровые компоненты включены!");
    }
    
    void HandleInput()
    {
        // Pause/Resume
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        
        // Restart
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
        
        // Start game
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
    }
    
    void CheckGameState()
    {
        if (playerHealth != null && playerHealth.IsDead())
        {
            // Game Over
            Debug.Log("Game Over!");
            Invoke(nameof(RestartGame), 3f);
        }
    }
    
    public void TogglePause()
    {
        isGamePaused = !isGamePaused;
        Time.timeScale = isGamePaused ? 0f : 1f;
        Debug.Log(isGamePaused ? "Игра на паузе" : "Игра возобновлена");
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    public void StartGame()
    {
        // Включаем игровые компоненты
        EnableGameComponents();
        
        // Запускаем время
        Time.timeScale = 1f;
        
        Debug.Log("Игра запущена!");
    }
} 