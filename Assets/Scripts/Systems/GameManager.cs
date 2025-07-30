using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private bool isGamePaused = false;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject mainMenu;
    
    [Header("Player References")]
    [SerializeField] private TankController player;
    [SerializeField] private HealthSystem playerHealth;
    
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
        if (player == null)
            player = FindObjectOfType<TankController>();
            
        if (playerHealth == null && player != null)
            playerHealth = player.GetComponent<HealthSystem>();
            
        // Отключаем игровые компоненты до старта игры
        DisableGameComponents();
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
        // Отключаем все танки до старта игры
        TankController[] tanks = FindObjectsOfType<TankController>();
        foreach (TankController tank in tanks)
        {
            if (tank != null)
                tank.enabled = false;
        }
        
        // Отключаем ИИ врагов
        EnemyAIController[] enemies = FindObjectsOfType<EnemyAIController>();
        foreach (EnemyAIController enemy in enemies)
        {
            if (enemy != null)
                enemy.enabled = false;
        }
        
        Debug.Log("Игровые компоненты отключены до старта!");
    }
    
    public void EnableGameComponents()
    {
        // Включаем все танки
        TankController[] tanks = FindObjectsOfType<TankController>();
        foreach (TankController tank in tanks)
        {
            if (tank != null)
                tank.enabled = true;
        }
        
        // Включаем ИИ врагов
        EnemyAIController[] enemies = FindObjectsOfType<EnemyAIController>();
        foreach (EnemyAIController enemy in enemies)
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
        
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(isGamePaused);
        }
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
} 