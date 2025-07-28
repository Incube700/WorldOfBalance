using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using WorldOfBalance.Systems;
using WorldOfBalance.Player;

namespace WorldOfBalance.UI
{
    /// <summary>
    /// Игровой HUD для PvP-игры "Мир Баланса"
    /// Отображает здоровье, статус игры и информацию о подключении
    /// </summary>
    public class GameHUD : MonoBehaviour
    {
        [Header("Health UI")]
        [SerializeField] private Slider healthBar;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private Image healthBarFill;
        
        [Header("Game Status")]
        [SerializeField] private TextMeshProUGUI gameStatusText;
        [SerializeField] private TextMeshProUGUI playerCountText;
        [SerializeField] private TextMeshProUGUI connectionStatusText;
        
        [Header("Settings")]
        [SerializeField] private Color fullHealthColor = Color.green;
        [SerializeField] private Color lowHealthColor = Color.red;
        [SerializeField] private Color mediumHealthColor = Color.yellow;
        
        private NetworkPlayerController localPlayer;
        private HealthSystem localPlayerHealth;
        
        private void Start()
        {
            // Находим локального игрока
            FindLocalPlayer();
            
            // Настройка цветов здоровья
            if (healthBarFill != null)
            {
                healthBarFill.color = fullHealthColor;
            }
        }
        
        private void Update()
        {
            // Поиск локального игрока, если он еще не найден
            if (localPlayer == null)
            {
                FindLocalPlayer();
            }
            
            // Обновление UI
            UpdateHealthUI();
            UpdateGameStatus();
            UpdateConnectionStatus();
        }
        
        /// <summary>
        /// Поиск локального игрока
        /// </summary>
        private void FindLocalPlayer()
        {
            NetworkPlayerController[] players = FindObjectsOfType<NetworkPlayerController>();
            
            foreach (var player in players)
            {
                if (player.IsLocalPlayer)
                {
                    localPlayer = player;
                    localPlayerHealth = player.GetComponent<HealthSystem>();
                    
                    // Подписываемся на события здоровья
                    if (localPlayerHealth != null)
                    {
                        localPlayerHealth.OnHealthChanged += OnHealthChanged;
                        localPlayerHealth.OnPlayerDied += OnPlayerDied;
                    }
                    
                    Debug.Log("Локальный игрок найден!");
                    break;
                }
            }
        }
        
        /// <summary>
        /// Обновление UI здоровья
        /// </summary>
        private void UpdateHealthUI()
        {
            if (localPlayerHealth == null) return;
            
            float healthPercentage = localPlayerHealth.HealthPercentage;
            
            // Обновление слайдера здоровья
            if (healthBar != null)
            {
                healthBar.value = healthPercentage;
            }
            
            // Обновление текста здоровья
            if (healthText != null)
            {
                healthText.text = $"{Mathf.Ceil(localPlayerHealth.CurrentHealth)}/{localPlayerHealth.MaxHealth}";
            }
            
            // Обновление цвета полосы здоровья
            if (healthBarFill != null)
            {
                if (healthPercentage > 0.6f)
                {
                    healthBarFill.color = fullHealthColor;
                }
                else if (healthPercentage > 0.3f)
                {
                    healthBarFill.color = mediumHealthColor;
                }
                else
                {
                    healthBarFill.color = lowHealthColor;
                }
            }
        }
        
        /// <summary>
        /// Обновление статуса игры
        /// </summary>
        private void UpdateGameStatus()
        {
            if (gameStatusText != null)
            {
                if (localPlayer != null && localPlayer.IsDead)
                {
                    gameStatusText.text = "ВЫ ПОГИБЛИ";
                    gameStatusText.color = Color.red;
                }
                else
                {
                    gameStatusText.text = "ИГРА АКТИВНА";
                    gameStatusText.color = Color.green;
                }
            }
            
            // Обновление количества игроков
            if (playerCountText != null)
            {
                NetworkPlayerController[] players = FindObjectsOfType<NetworkPlayerController>();
                int alivePlayers = 0;
                
                foreach (var player in players)
                {
                    if (!player.IsDead)
                    {
                        alivePlayers++;
                    }
                }
                
                playerCountText.text = $"Игроков: {alivePlayers}/{players.Length}";
            }
        }
        
        /// <summary>
        /// Обновление статуса подключения
        /// </summary>
        private void UpdateConnectionStatus()
        {
            if (connectionStatusText != null)
            {
                if (NetworkServer.active && NetworkClient.active)
                {
                    connectionStatusText.text = "Хост";
                    connectionStatusText.color = Color.blue;
                }
                else if (NetworkClient.active)
                {
                    connectionStatusText.text = "Клиент";
                    connectionStatusText.color = Color.green;
                }
                else if (NetworkServer.active)
                {
                    connectionStatusText.text = "Сервер";
                    connectionStatusText.color = Color.yellow;
                }
                else
                {
                    connectionStatusText.text = "Не подключен";
                    connectionStatusText.color = Color.red;
                }
            }
        }
        
        /// <summary>
        /// Обработка изменения здоровья
        /// </summary>
        private void OnHealthChanged(float newHealth)
        {
            UpdateHealthUI();
        }
        
        /// <summary>
        /// Обработка смерти игрока
        /// </summary>
        private void OnPlayerDied()
        {
            UpdateGameStatus();
            Debug.Log("Локальный игрок погиб!");
        }
        
        /// <summary>
        /// Показать информацию о здоровье в GUI (для отладки)
        /// </summary>
        private void OnGUI()
        {
            if (localPlayerHealth != null)
            {
                GUI.Label(new Rect(10, 30, 200, 20), $"Health: {localPlayerHealth.CurrentHealth}/{localPlayerHealth.MaxHealth}");
                GUI.Label(new Rect(10, 50, 200, 20), $"Is Dead: {localPlayerHealth.IsDead}");
            }
            
            if (localPlayer != null)
            {
                GUI.Label(new Rect(10, 70, 200, 20), $"Is Local Player: {localPlayer.IsLocalPlayer}");
            }
        }
        
        private void OnDestroy()
        {
            // Отписываемся от событий
            if (localPlayerHealth != null)
            {
                localPlayerHealth.OnHealthChanged -= OnHealthChanged;
                localPlayerHealth.OnPlayerDied -= OnPlayerDied;
            }
        }
    }
} 