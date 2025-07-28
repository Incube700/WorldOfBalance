using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("Player 1 UI")]
    public Slider player1HealthBar;
    public Slider player1ArmorBar;
    public TextMeshProUGUI player1HealthText;
    public TextMeshProUGUI player1ArmorText;
    public Image player1ArmorZoneIndicator;
    
    [Header("Player 2 UI")]
    public Slider player2HealthBar;
    public Slider player2ArmorBar;
    public TextMeshProUGUI player2HealthText;
    public TextMeshProUGUI player2ArmorText;
    public Image player2ArmorZoneIndicator;
    
    [Header("Game Info")]
    public TextMeshProUGUI matchTimerText;
    public TextMeshProUGUI gameStatusText;
    
    [Header("Colors")]
    public Color frontArmorColor = Color.green;
    public Color sideArmorColor = Color.yellow;
    public Color backArmorColor = Color.red;
    
    private PlayerController player1;
    private PlayerController player2;
    private GameManager gameManager;
    
    void Start()
    {
        gameManager = GameManager.Instance;
        
        // Находим игроков
        GameObject p1 = GameObject.FindGameObjectWithTag("Player");
        if (p1 != null) player1 = p1.GetComponent<PlayerController>();
        
        GameObject p2 = GameObject.FindGameObjectWithTag("Player2");
        if (p2 != null) player2 = p2.GetComponent<PlayerController>();
        
        InitializeUI();
    }
    
    void Update()
    {
        UpdatePlayerUI();
        UpdateGameInfo();
    }
    
    void InitializeUI()
    {
        // Инициализируем слайдеры здоровья
        if (player1HealthBar != null)
        {
            player1HealthBar.minValue = 0f;
            player1HealthBar.maxValue = 100f;
        }
        
        if (player2HealthBar != null)
        {
            player2HealthBar.minValue = 0f;
            player2HealthBar.maxValue = 100f;
        }
        
        // Инициализируем слайдеры брони
        if (player1ArmorBar != null)
        {
            player1ArmorBar.minValue = 0f;
            player1ArmorBar.maxValue = 50f;
        }
        
        if (player2ArmorBar != null)
        {
            player2ArmorBar.minValue = 0f;
            player2ArmorBar.maxValue = 50f;
        }
    }
    
    void UpdatePlayerUI()
    {
        // Обновляем UI игрока 1
        if (player1 != null)
        {
            UpdatePlayerHealthUI(player1, player1HealthBar, player1HealthText, player1ArmorBar, player1ArmorText, player1ArmorZoneIndicator);
        }
        
        // Обновляем UI игрока 2
        if (player2 != null)
        {
            UpdatePlayerHealthUI(player2, player2HealthBar, player2HealthText, player2ArmorBar, player2ArmorText, player2ArmorZoneIndicator);
        }
    }
    
    void UpdatePlayerHealthUI(PlayerController player, Slider healthBar, TextMeshProUGUI healthText, Slider armorBar, TextMeshProUGUI armorText, Image armorZoneIndicator)
    {
        if (healthBar != null)
        {
            healthBar.value = player.hp;
        }
        
        if (healthText != null)
        {
            healthText.text = $"HP: {player.hp:F0}";
        }
        
        if (armorBar != null)
        {
            armorBar.value = player.maxArmor;
        }
        
        if (armorText != null)
        {
            armorText.text = $"Armor: {player.maxArmor:F0}";
        }
        
        // Обновляем индикатор зоны брони на основе поворота игрока
        if (armorZoneIndicator != null)
        {
            UpdateArmorZoneIndicator(player, armorZoneIndicator);
        }
    }
    
    void UpdateArmorZoneIndicator(PlayerController player, Image indicator)
    {
        // Получаем угол поворота игрока
        float playerRotation = player.transform.eulerAngles.z;
        
        // Определяем, какая зона брони активна
        float normalizedRotation = (playerRotation + 360) % 360;
        
        if (normalizedRotation <= 45 || normalizedRotation >= 315)
        {
            // Передняя броня
            indicator.color = frontArmorColor;
        }
        else if ((normalizedRotation > 45 && normalizedRotation <= 135) || (normalizedRotation >= 225 && normalizedRotation < 315))
        {
            // Боковая броня
            indicator.color = sideArmorColor;
        }
        else
        {
            // Задняя броня
            indicator.color = backArmorColor;
        }
    }
    
    void UpdateGameInfo()
    {
        if (gameManager != null)
        {
            // Обновляем таймер матча
            if (matchTimerText != null)
            {
                matchTimerText.text = gameManager.GetFormattedTime();
            }
            
            // Обновляем статус игры
            if (gameStatusText != null)
            {
                if (gameManager.IsGamePaused())
                {
                    gameStatusText.text = "PAUSED";
                    gameStatusText.color = Color.yellow;
                }
                else if (gameManager.IsGameOver())
                {
                    gameStatusText.text = "GAME OVER";
                    gameStatusText.color = Color.red;
                }
                else
                {
                    gameStatusText.text = "FIGHTING";
                    gameStatusText.color = Color.green;
                }
            }
        }
    }
    
    // Методы для кнопок UI
    public void OnPauseButton()
    {
        if (gameManager != null)
        {
            gameManager.TogglePause();
        }
    }
    
    public void OnRestartButton()
    {
        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
    }
    
    public void OnQuitButton()
    {
        if (gameManager != null)
        {
            gameManager.QuitGame();
        }
    }
} 