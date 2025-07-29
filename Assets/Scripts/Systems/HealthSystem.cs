using UnityEngine;
using Mirror;
using System;

namespace WorldOfBalance.Systems
{
    public class HealthSystem : NetworkBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;
        
        // Network synchronization
        [SyncVar] private float networkHealth;
        
        // Events
        public event Action<float> OnHealthChanged;
        public event Action OnPlayerDied;
        public event Action OnPlayerRespawned;
        
        // Properties
        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public float HealthPercentage => maxHealth > 0 ? currentHealth / maxHealth : 0f;
        public bool IsDead => currentHealth <= 0f;
        
        private void Awake()
        {
            // Получаем компоненты с типизацией
            NetworkPlayerController playerController = GetComponent<NetworkPlayerController>();
            if (playerController == null)
            {
                Debug.LogWarning("HealthSystem: NetworkPlayerController not found!");
            }
            
            // Инициализируем здоровье
            if (isServer)
            {
                ResetHealth();
            }
        }
        
        /// <summary>
        /// Сбрасывает здоровье к максимальному значению
        /// </summary>
        [Server]
        public void ResetHealth()
        {
            currentHealth = maxHealth;
            networkHealth = currentHealth;
            RpcUpdateHealth(currentHealth);
        }
        
        /// <summary>
        /// Наносит урон игроку
        /// </summary>
        /// <param name="damage">Количество урона</param>
        [Server]
        public void TakeDamage(float damage)
        {
            if (IsDead) return;
            
            currentHealth = Mathf.Max(0f, currentHealth - damage);
            networkHealth = currentHealth;
            
            RpcUpdateHealth(currentHealth);
            
            Debug.Log($"Player took {damage} damage. Health: {currentHealth}/{maxHealth}");
            
            if (IsDead)
            {
                OnPlayerDied?.Invoke();
                RpcPlayerDied();
            }
        }
        
        /// <summary>
        /// Восстанавливает здоровье игрока
        /// </summary>
        /// <param name="healAmount">Количество восстанавливаемого здоровья</param>
        [Server]
        public void Heal(float healAmount)
        {
            if (IsDead) return;
            
            currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
            networkHealth = currentHealth;
            
            RpcUpdateHealth(currentHealth);
            
            Debug.Log($"Player healed {healAmount} health. Health: {currentHealth}/{maxHealth}");
        }
        
        /// <summary>
        /// Устанавливает здоровье игрока
        /// </summary>
        /// <param name="health">Новое значение здоровья</param>
        [Server]
        public void SetHealth(float health)
        {
            currentHealth = Mathf.Clamp(health, 0f, maxHealth);
            networkHealth = currentHealth;
            
            RpcUpdateHealth(currentHealth);
            
            if (IsDead)
            {
                OnPlayerDied?.Invoke();
                RpcPlayerDied();
            }
        }
        
        /// <summary>
        /// Устанавливает максимальное здоровье
        /// </summary>
        /// <param name="newMaxHealth">Новое максимальное здоровье</param>
        [Server]
        public void SetMaxHealth(float newMaxHealth)
        {
            maxHealth = newMaxHealth;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
            networkHealth = currentHealth;
            
            RpcUpdateHealth(currentHealth);
        }
        
        /// <summary>
        /// Обновляет здоровье на клиентах
        /// </summary>
        /// <param name="health">Текущее здоровье</param>
        [ClientRpc]
        private void RpcUpdateHealth(float health)
        {
            currentHealth = health;
            OnHealthChanged?.Invoke(currentHealth);
        }
        
        /// <summary>
        /// Уведомляет клиентов о смерти игрока
        /// </summary>
        [ClientRpc]
        private void RpcPlayerDied()
        {
            OnPlayerDied?.Invoke();
            Debug.Log("Player died on client!");
        }
        
        /// <summary>
        /// Уведомляет клиентов о возрождении игрока
        /// </summary>
        [ClientRpc]
        public void RpcPlayerRespawned()
        {
            OnPlayerRespawned?.Invoke();
            Debug.Log("Player respawned on client!");
        }
        
        /// <summary>
        /// Возрождает игрока
        /// </summary>
        [Server]
        public void Respawn()
        {
            ResetHealth();
            RpcPlayerRespawned();
        }
        
        /// <summary>
        /// Получает информацию о здоровье
        /// </summary>
        /// <returns>Кортеж с информацией о здоровье (current, max, percentage)</returns>
        public (float current, float max, float percentage) GetHealthInfo()
        {
            return (currentHealth, maxHealth, HealthPercentage);
        }
        
        /// <summary>
        /// Проверяет, находится ли игрок в критическом состоянии
        /// </summary>
        /// <param name="threshold">Порог критического здоровья (по умолчанию 25%)</param>
        /// <returns>true, если здоровье ниже порога</returns>
        public bool IsCriticalHealth(float threshold = 0.25f)
        {
            return HealthPercentage <= threshold;
        }
    }
} 