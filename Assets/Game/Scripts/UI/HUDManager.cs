using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WorldOfBalance.UI
{
    /// <summary>
    /// Менеджер пользовательского интерфейса (HUD) для отображения здоровья, брони и боеприпасов.
    /// Автоматически связывается с системами танка для обновления в реальном времени.
    /// </summary>
    public class HUDManager : MonoBehaviour
    {
        [Header("Health UI")]
        [SerializeField] private Slider healthBar;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private Image healthBarFill;
        [SerializeField] private Color healthColorHigh = Color.green;
        [SerializeField] private Color healthColorMedium = Color.yellow;
        [SerializeField] private Color healthColorLow = Color.red;
        
        [Header("Armor UI")]
        [SerializeField] private Slider armorBar;
        [SerializeField] private TextMeshProUGUI armorText;
        [SerializeField] private Image armorBarFill;
        [SerializeField] private Color armorColor = Color.blue;
        
        [Header("Ammo UI")]
        [SerializeField] private TextMeshProUGUI ammoText;
        [SerializeField] private Image ammoIcon;
        
        [Header("Game Info")]
        [SerializeField] private TextMeshProUGUI gameTimeText;
        [SerializeField] private TextMeshProUGUI scoreText;
        
        [Header("Target Settings")]
        [SerializeField] private bool findPlayerAutomatically = true;
        
        // Target references
        private HealthSystem targetHealthSystem;
        private TankController targetTankController;
        private PlayerController targetPlayerController;
        
        // Game state
        private float gameStartTime;
        private int currentScore = 0;
        
        void Start()
        {
            gameStartTime = Time.time;
            InitializeHUD();
            
            if (findPlayerAutomatically)
            {
                FindPlayerTarget();
            }
        }
        
        void Update()
        {
            UpdateHUD();
            UpdateGameTime();
        }
        
        void InitializeHUD()
        {
            // Initialize health bar
            if (healthBar != null)
            {
                healthBar.minValue = 0;
                healthBar.maxValue = 100;
                healthBar.value = 100;
            }
            
            // Initialize armor bar
            if (armorBar != null)
            {
                armorBar.minValue = 0;
                armorBar.maxValue = 100;
                armorBar.value = 100;
            }
            
            // Set initial colors
            if (healthBarFill != null)
                healthBarFill.color = healthColorHigh;
                
            if (armorBarFill != null)
                armorBarFill.color = armorColor;
            
            Debug.Log("HUDManager: HUD initialized");
        }
        
        void FindPlayerTarget()
        {
            // Try to find player by tag
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                SetTarget(player);
                return;
            }
            
            // Try to find by PlayerController component
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                SetTarget(playerController.gameObject);
                return;
            }
            
            // Try to find by TankController marked as player
            TankController[] tanks = FindObjectsOfType<TankController>();
            foreach (var tank in tanks)
            {
                // Assuming player tank has specific name or is marked somehow
                if (tank.name.ToLower().Contains("player"))
                {
                    SetTarget(tank.gameObject);
                    return;
                }
            }
            
            Debug.LogWarning("HUDManager: No player target found for HUD");
        }
        
        /// <summary>
        /// Устанавливает целевой объект для отслеживания статистики
        /// </summary>
        /// <param name="target">Целевой игровой объект</param>
        public void SetTarget(GameObject target)
        {
            if (target == null) return;
            
            targetHealthSystem = target.GetComponent<HealthSystem>();
            targetTankController = target.GetComponent<TankController>();
            targetPlayerController = target.GetComponent<PlayerController>();
            
            Debug.Log($"HUDManager: Target set to {target.name}");
        }
        
        void UpdateHUD()
        {
            UpdateHealthUI();
            UpdateArmorUI();
            UpdateAmmoUI();
        }
        
        void UpdateHealthUI()
        {
            float currentHealth = 0;
            float maxHealth = 100;
            
            // Get health from different sources
            if (targetHealthSystem != null)
            {
                currentHealth = targetHealthSystem.CurrentHealth;
                maxHealth = targetHealthSystem.MaxHealth;
            }
            else if (targetTankController != null)
            {
                currentHealth = targetTankController.Health;
                maxHealth = targetTankController.MaxHealth;
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
                healthText.text = $"{currentHealth:F0}/{maxHealth:F0}";
            }
            
            // Update health bar color based on percentage
            if (healthBarFill != null)
            {
                float healthPercent = currentHealth / maxHealth;
                
                if (healthPercent > 0.6f)
                    healthBarFill.color = healthColorHigh;
                else if (healthPercent > 0.3f)
                    healthBarFill.color = healthColorMedium;
                else
                    healthBarFill.color = healthColorLow;
            }
        }
        
        void UpdateArmorUI()
        {
            float currentArmor = 0;
            float maxArmor = 100;
            
            // Get armor from tank controller
            if (targetTankController != null)
            {
                currentArmor = targetTankController.Armor;
                maxArmor = targetTankController.Armor; // Assuming armor doesn't change
            }
            
            // Update armor bar
            if (armorBar != null)
            {
                armorBar.maxValue = maxArmor;
                armorBar.value = currentArmor;
            }
            
            // Update armor text
            if (armorText != null)
            {
                armorText.text = $"Armor: {currentArmor:F0}";
            }
        }
        
        void UpdateAmmoUI()
        {
            // For now, show infinite ammo (can be extended for limited ammo system)
            if (ammoText != null)
            {
                ammoText.text = "∞";
            }
        }
        
        void UpdateGameTime()
        {
            if (gameTimeText != null)
            {
                float currentTime = Time.time - gameStartTime;
                int minutes = Mathf.FloorToInt(currentTime / 60);
                int seconds = Mathf.FloorToInt(currentTime % 60);
                gameTimeText.text = $"{minutes:00}:{seconds:00}";
            }
        }
        
        /// <summary>
        /// Обновляет счет игрока
        /// </summary>
        /// <param name="newScore">Новый счет</param>
        public void UpdateScore(int newScore)
        {
            currentScore = newScore;
            
            if (scoreText != null)
            {
                scoreText.text = $"Score: {currentScore}";
            }
        }
        
        /// <summary>
        /// Добавляет очки к текущему счету
        /// </summary>
        /// <param name="points">Количество очков для добавления</param>
        public void AddScore(int points)
        {
            currentScore += points;
            UpdateScore(currentScore);
        }
        
        /// <summary>
        /// Показывает сообщение на HUD (можно расширить для уведомлений)
        /// </summary>
        /// <param name="message">Сообщение для отображения</param>
        public void ShowMessage(string message)
        {
            Debug.Log($"HUD Message: {message}");
            // TODO: Implement message display system
        }
        
        /// <summary>
        /// Сбрасывает HUD к начальному состоянию
        /// </summary>
        public void ResetHUD()
        {
            gameStartTime = Time.time;
            currentScore = 0;
            
            if (healthBar != null) healthBar.value = healthBar.maxValue;
            if (armorBar != null) armorBar.value = armorBar.maxValue;
            if (healthBarFill != null) healthBarFill.color = healthColorHigh;
            
            UpdateScore(0);
        }
    }
}