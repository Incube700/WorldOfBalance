using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Simple game manager for minimalistic tank duel game.
/// Handles game states, win/lose conditions, and UI.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private TankController playerTank;
    [SerializeField] private TankController enemyTank;
    [SerializeField] private Camera gameCamera;
    
    [Header("UI")]
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private UnityEngine.UI.Text gameOverText;
    [SerializeField] private UnityEngine.UI.Text playerHealthText;
    [SerializeField] private UnityEngine.UI.Text enemyHealthText;
    
    // Game state
    private bool gameStarted = false;
    private bool gameEnded = false;
    
    void Start()
    {
        // Find tanks if not assigned
        if (playerTank == null)
            playerTank = GameObject.FindGameObjectWithTag("Player")?.GetComponent<TankController>();
        
        if (enemyTank == null)
            enemyTank = GameObject.FindGameObjectWithTag("Enemy")?.GetComponent<TankController>();
        
        // Reset tank positions to prevent wall collision
        ResetTankPositions();
        
        // Setup camera
        if (gameCamera == null)
            gameCamera = Camera.main;
        
        if (gameCamera != null)
        {
            gameCamera.orthographic = true;
            gameCamera.orthographicSize = 8f;
            gameCamera.transform.position = new Vector3(0, 0, -10);
        }
        
        // Start game immediately (no menu for now)
        StartGame();
        
        Debug.Log("GameManager initialized");
    }
    
    void ResetTankPositions()
    {
        // Reset player tank position
        if (playerTank != null)
        {
            playerTank.transform.position = new Vector3(-4f, 0f, 0f);
            playerTank.transform.rotation = Quaternion.identity;
        }
        
        // Reset enemy tank position  
        if (enemyTank != null)
        {
            enemyTank.transform.position = new Vector3(4f, 0f, 0f);
            enemyTank.transform.rotation = Quaternion.identity;
        }
        
        Debug.Log("Tank positions reset");
    }
    
    void Update()
    {
        if (!gameStarted || gameEnded) return;
        
        // Update UI
        UpdateHealthUI();
        
        // Check win/lose conditions
        CheckGameEnd();
        
        // Quit game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }
    
    void UpdateHealthUI()
    {
        if (playerHealthText != null && playerTank != null)
        {
            playerHealthText.text = $"Player: {playerTank.Health:F0}/{playerTank.MaxHealth:F0}";
        }
        
        if (enemyHealthText != null && enemyTank != null)
        {
            enemyHealthText.text = $"Enemy: {enemyTank.Health:F0}/{enemyTank.MaxHealth:F0}";
        }
    }
    
    void CheckGameEnd()
    {
        if (playerTank != null && playerTank.IsDead)
        {
            EndGame("DEFEAT", "Enemy tank destroyed you!");
        }
        else if (enemyTank != null && enemyTank.IsDead)
        {
            EndGame("VICTORY", "You destroyed the enemy tank!");
        }
    }
    
    public void StartGame()
    {
        gameStarted = true;
        gameEnded = false;
        
        // Hide start menu
        if (startMenu != null)
            startMenu.SetActive(false);
        
        // Hide game over menu
        if (gameOverMenu != null)
            gameOverMenu.SetActive(false);
        
        // Enable tanks
        if (playerTank != null)
            playerTank.enabled = true;
        
        if (enemyTank != null)
            enemyTank.enabled = true;
        
        Debug.Log("Game started!");
    }
    
    void EndGame(string result, string message)
    {
        if (gameEnded) return;
        
        gameEnded = true;
        
        // Disable tanks
        if (playerTank != null)
            playerTank.enabled = false;
        
        if (enemyTank != null)
            enemyTank.enabled = false;
        
        // Show game over menu
        if (gameOverMenu != null)
        {
            gameOverMenu.SetActive(true);
            
            if (gameOverText != null)
            {
                gameOverText.text = $"{result}\n{message}";
            }
        }
        
        Debug.Log($"Game ended: {result} - {message}");
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    void ShowStartMenu()
    {
        gameStarted = false;
        gameEnded = false;
        
        if (startMenu != null)
            startMenu.SetActive(true);
        
        if (gameOverMenu != null)
            gameOverMenu.SetActive(false);
        
        // Disable tanks until game starts
        if (playerTank != null)
            playerTank.enabled = false;
        
        if (enemyTank != null)
            enemyTank.enabled = false;
    }
    
    // Helper method to create UI programmatically if needed
    void CreateDefaultUI()
    {
        // Create Canvas
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        
        // Create Start Menu
        GameObject startMenuObj = new GameObject("StartMenu");
        startMenuObj.transform.SetParent(canvasObj.transform);
        
        // Create Game Over Menu
        GameObject gameOverMenuObj = new GameObject("GameOverMenu");
        gameOverMenuObj.transform.SetParent(canvasObj.transform);
        gameOverMenuObj.SetActive(false);
        
        startMenu = startMenuObj;
        gameOverMenu = gameOverMenuObj;
        
        Debug.Log("Default UI created");
    }
}