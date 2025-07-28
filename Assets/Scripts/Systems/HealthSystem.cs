using UnityEngine;
using Mirror;
using WorldOfBalance.Player;

namespace WorldOfBalance.Systems
{
    /// <summary>
    /// Система здоровья для игроков в PvP-игре "Мир Баланса"
    /// Управляет здоровьем, смертью и возрождением игроков
    /// </summary>
    public class HealthSystem : NetworkBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;
        
        [Header("UI References")]
        [SerializeField] private UnityEngine.UI.Slider healthBar;
        [SerializeField] private UnityEngine.UI.Text healthText;
        
        // Сетевые переменные
        [SyncVar] private float networkHealth;
        
        // События
        public System.Action<float> OnHealthChanged;
        public System.Action OnPlayerDied;
        public System.Action OnPlayerRespawned;
        
        // Свойства
        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;
        public float HealthPercentage => currentHealth / maxHealth;
        public bool IsDead => currentHealth <= 0f;
        
        private NetworkPlayerController playerController;
        
        private void Awake()
        {
            playerController = GetComponent<NetworkPlayerController>();
        }
        
        private void Start()
        {
            if (isServer)
            {
                ResetHealth();
            }
        }
        
        /// <summary>
        /// Сброс здоровья к максимальному значению
        /// </summary>
        [Server]
        public void ResetHealth()
        {
            currentHealth = maxHealth;
            networkHealth = currentHealth;
            UpdateHealthUI();
        }
        
        /// <summary>
        /// Получение урона
        /// </summary>
        [Server]
        public void TakeDamage(float damage)
        {
            if (IsDead) return;
            
            currentHealth = Mathf.Max(0f, currentHealth - damage);
            networkHealth = currentHealth;
            
            // Уведомление клиентов об изменении здоровья
            RpcUpdateHealth(currentHealth);
            
            // Проверка смерти
            if (IsDead)
            {
                OnPlayerDied?.Invoke();
                RpcPlayerDied();
            }
        }
        
        /// <summary>
        /// Восстановление здоровья
        /// </summary>
        [Server]
        public void Heal(float amount)
        {
            if (IsDead) return;
            
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            networkHealth = currentHealth;
            
            RpcUpdateHealth(currentHealth);
        }
        
        /// <summary>
        /// Установка здоровья (для отладки)
        /// </summary>
        [Server]
        public void SetHealth(float health)
        {
            currentHealth = Mathf.Clamp(health, 0f, maxHealth);
            networkHealth = currentHealth;
            
            RpcUpdateHealth(currentHealth);
        }
        
        /// <summary>
        /// Обновление здоровья на клиентах
        /// </summary>
        [ClientRpc]
        private void RpcUpdateHealth(float newHealth)
        {
            currentHealth = newHealth;
            UpdateHealthUI();
            OnHealthChanged?.Invoke(currentHealth);
        }
        
        /// <summary>
        /// Уведомление о смерти игрока
        /// </summary>
        [ClientRpc]
        private void RpcPlayerDied()
        {
            Debug.Log($"Игрок {gameObject.name} погиб!");
            
            // Отключение компонентов
            if (playerController != null)
            {
                playerController.RpcDie();
            }
            
            OnPlayerDied?.Invoke();
        }
        
        /// <summary>
        /// Обновление UI здоровья
        /// </summary>
        private void UpdateHealthUI()
        {
            if (healthBar != null)
            {
                healthBar.value = HealthPercentage;
            }
            
            if (healthText != null)
            {
                healthText.text = $"{Mathf.Ceil(currentHealth)}/{maxHealth}";
            }
        }
        
        /// <summary>
        /// Получение информации о здоровье для отладки
        /// </summary>
        private void OnGUI()
        {
            if (isLocalPlayer)
            {
                GUI.Label(new Rect(10, 10, 200, 20), $"Health: {currentHealth}/{maxHealth}");
            }
        }
    }
} 