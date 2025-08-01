using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Простой HUD для WorldOfBalance - показывает основную информацию
/// </summary>
public class SimpleHUD : MonoBehaviour
{
    [Header("Health UI")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    
    [Header("Game Info")]
    [SerializeField] private TextMeshProUGUI gameTimeText;
    [SerializeField] private TextMeshProUGUI statusText;
    
    [Header("Game Over UI")]
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI winnerText;
    
    // Target player
    private PlayerController player;
    private TankController playerTank;
    
    void Start()
    {
        FindPlayer();
        
        // Initialize UI
        if (healthBar != null)
        {
            healthBar.minValue = 0;
            healthBar.maxValue = 100;
            healthBar.value = 100;
        }
        
        if (statusText != null)
        {
            statusText.text = "Ready for Battle!";
        }
    }
    
    void Update()
    {
        UpdateHealthUI();
        UpdateGameTimeUI();
        UpdateGameStatusUI();
    }
    
    void FindPlayer()
    {
        // Try to find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            playerObj = GameObject.Find("Player");
        }
        
        if (playerObj != null)
        {
            player = playerObj.GetComponent<PlayerController>();
            playerTank = playerObj.GetComponent<TankController>();
            Debug.Log($"SimpleHUD: Found player - {playerObj.name}");
        }
        else
        {
            Debug.LogWarning("SimpleHUD: No player found!");
        }
    }
    
    void UpdateHealthUI()
    {
        float currentHealth = 100f;
        float maxHealth = 100f;
        
        // Get health from player
        if (playerTank != null)
        {
            currentHealth = playerTank.Health;
            maxHealth = playerTank.MaxHealth;
        }
        else if (player != null)
        {
            // Try to get health from PlayerController if it has health
            HealthSystem healthSystem = player.GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                currentHealth = healthSystem.CurrentHealth;
                maxHealth = healthSystem.MaxHealth;
            }
        }
        
        // Update health bar
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
        
        // Update health text
        if (healthText != null)
        {
            healthText.text = $"Health: {currentHealth:F0}/{maxHealth:F0}";
        }
    }
    
    void UpdateGameTimeUI()
    {
        if (gameTimeText != null && SimpleGameManager.Instance != null)
        {
            float gameTime = SimpleGameManager.Instance.GetGameTime();
            int minutes = Mathf.FloorToInt(gameTime / 60);
            int seconds = Mathf.FloorToInt(gameTime % 60);
            gameTimeText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }
    
    void UpdateGameStatusUI()
    {
        if (SimpleGameManager.Instance == null) return;
        
        GameState currentState = SimpleGameManager.Instance.CurrentState;
        
        if (statusText != null)
        {
            switch (currentState)
            {
                case GameState.MainMenu:
                    statusText.text = "Press PLAY to start battle!";
                    break;
                    
                case GameState.Playing:
                    statusText.text = "BATTLE IN PROGRESS";
                    break;
                    
                case GameState.GameOver:
                    statusText.text = "GAME OVER";
                    break;
                    
                case GameState.Victory:
                    statusText.text = "VICTORY!";
                    break;
            }
        }
        
        // Update game over UI
        if (gameOverText != null)
        {
            if (currentState == GameState.GameOver)
            {
                gameOverText.text = "DEFEAT";
                gameOverText.gameObject.SetActive(true);
            }
            else if (currentState == GameState.Victory)
            {
                gameOverText.text = "VICTORY";
                gameOverText.gameObject.SetActive(true);
            }
            else
            {
                gameOverText.gameObject.SetActive(false);
            }
        }
        
        // Update winner text
        if (winnerText != null)
        {
            if (currentState == GameState.GameOver || currentState == GameState.Victory)
            {
                string winner = SimpleGameManager.Instance.GetWinner();
                winnerText.text = $"Winner: {winner}";
                winnerText.gameObject.SetActive(true);
            }
            else
            {
                winnerText.gameObject.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// Показать сообщение на экране
    /// </summary>
    public void ShowMessage(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
        
        Debug.Log($"SimpleHUD: {message}");
    }
}