using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WorldOfBalance.Player;
using WorldOfBalance.Systems;

namespace WorldOfBalance.UI
{
    public class GameHUD : MonoBehaviour
    {
        [Header("Health UI")]
        [SerializeField] private Slider healthBar;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private Image healthBarFill;
        
        [Header("Game Info")]
        [SerializeField] private TextMeshProUGUI playerCountText;
        [SerializeField] private TextMeshProUGUI connectionStatusText;
        [SerializeField] private TextMeshProUGUI gameTimeText;
        
        [Header("Colors")]
        [SerializeField] private Color fullHealthColor = Color.green;
        [SerializeField] private Color mediumHealthColor = Color.yellow;
        [SerializeField] private Color lowHealthColor = Color.red;
        
        // Local variables
        private HealthSystem localPlayerHealth;
        private NetworkPlayerController localPlayer;
        private NetworkGameManager gameManager;
        
        private void Start()
        {
            // Находим локального игрока
            FindLocalPlayer();
            
            // Находим менеджер игры
            gameManager = FindObjectOfType<NetworkGameManager>();
            
            // Подписываемся на события
            SubscribeToEvents();
        }
        
        /// <summary>
        /// Находит локального игрока
        /// </summary>
        private void FindLocalPlayer()
        {
            // Находим всех игроков с типизацией
            NetworkPlayerController[] players = FindObjectsOfType<NetworkPlayerController>();
            
            foreach (var player in players)
            {
                if (player.IsLocalPlayer)
                {
                    localPlayer = player;
                    localPlayerHealth = player.GetComponent<HealthSystem>();
                    
                    if (localPlayerHealth != null)
                    {
                        // Подписываемся на события здоровья
                        localPlayerHealth.OnHealthChanged += OnHealthChanged;
                        localPlayerHealth.OnPlayerDied += OnPlayerDied;
                        localPlayerHealth.OnPlayerRespawned += OnPlayerRespawned;
                    }
                    
                    Debug.Log("Local player found and subscribed to events");
                    break;
                }
            }
        }
        
        /// <summary>
        /// Подписывается на события игры
        /// </summary>
        private void SubscribeToEvents()
        {
            if (gameManager != null)
            {
                gameManager.OnGameOver += OnGameOver;
                gameManager.OnGameRestarted += OnGameRestarted;
            }
        }
        
        /// <summary>
        /// Отписывается от событий при уничтожении
        /// </summary>
        private void OnDestroy()
        {
            if (localPlayerHealth != null)
            {
                localPlayerHealth.OnHealthChanged -= OnHealthChanged;
                localPlayerHealth.OnPlayerDied -= OnPlayerDied;
                localPlayerHealth.OnPlayerRespawned -= OnPlayerRespawned;
            }
            
            if (gameManager != null)
            {
                gameManager.OnGameOver -= OnGameOver;
                gameManager.OnGameRestarted -= OnGameRestarted;
            }
        }
        
        private void Update()
        {
            UpdateGameInfo();
        }
        
        /// <summary>
        /// Обновляет информацию об игре
        /// </summary>
        private void UpdateGameInfo()
        {
            // Обновляем количество игроков
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
                playerCountText.text = $"Игроков: {alivePlayers}";
            }
            
            // Обновляем статус подключения
            if (connectionStatusText != null)
            {
                string status = "Отключено";
                if (NetworkServer.active && NetworkClient.active)
                    status = "Хост";
                else if (NetworkClient.active)
                    status = "Клиент";
                else if (NetworkServer.active)
                    status = "Сервер";
                
                connectionStatusText.text = $"Статус: {status}";
            }
            
            // Обновляем время игры
            if (gameTimeText != null && gameManager != null)
            {
                var matchInfo = gameManager.GetMatchInfo();
                int minutes = Mathf.FloorToInt(matchInfo.time / 60f);
                int seconds = Mathf.FloorToInt(matchInfo.time % 60f);
                gameTimeText.text = $"Время: {minutes:00}:{seconds:00}";
            }
        }
        
        /// <summary>
        /// Обработчик изменения здоровья
        /// </summary>
        /// <param name="newHealth">Новое значение здоровья</param>
        private void OnHealthChanged(float newHealth)
        {
            UpdateHealthUI();
        }
        
        /// <summary>
        /// Обработчик смерти игрока
        /// </summary>
        private void OnPlayerDied()
        {
            Debug.Log("Local player died!");
            UpdateHealthUI();
        }
        
        /// <summary>
        /// Обработчик возрождения игрока
        /// </summary>
        private void OnPlayerRespawned()
        {
            Debug.Log("Local player respawned!");
            UpdateHealthUI();
        }
        
        /// <summary>
        /// Обработчик окончания игры
        /// </summary>
        /// <param name="reason">Причина окончания</param>
        private void OnGameOver(string reason)
        {
            Debug.Log($"Game Over: {reason}");
            // Здесь можно показать экран окончания игры
        }
        
        /// <summary>
        /// Обработчик перезапуска игры
        /// </summary>
        private void OnGameRestarted()
        {
            Debug.Log("Game restarted!");
            FindLocalPlayer(); // Находим игрока заново
        }
        
        /// <summary>
        /// Обновляет UI здоровья
        /// </summary>
        private void UpdateHealthUI()
        {
            if (localPlayerHealth == null) return;
            
            float healthPercentage = localPlayerHealth.HealthPercentage;
            
            // Обновляем полосу здоровья
            if (healthBar != null)
            {
                healthBar.value = healthPercentage;
            }
            
            // Обновляем текст здоровья
            if (healthText != null)
            {
                healthText.text = $"{Mathf.Ceil(localPlayerHealth.CurrentHealth)}/{localPlayerHealth.MaxHealth}";
            }
            
            // Обновляем цвет полосы здоровья
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
        /// Получает информацию о здоровье
        /// </summary>
        /// <returns>Кортеж с информацией о здоровье</returns>
        public (float current, float max, float percentage) GetHealthInfo()
        {
            if (localPlayerHealth != null)
            {
                return localPlayerHealth.GetHealthInfo();
            }
            return (0f, 0f, 0f);
        }
        
        /// <summary>
        /// Получает информацию об игре
        /// </summary>
        /// <returns>Кортеж с информацией об игре</returns>
        public (int playerCount, string connectionStatus, float gameTime) GetGameInfo()
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
            
            string status = "Отключено";
            if (NetworkServer.active && NetworkClient.active)
                status = "Хост";
            else if (NetworkClient.active)
                status = "Клиент";
            else if (NetworkServer.active)
                status = "Сервер";
            
            float gameTime = 0f;
            if (gameManager != null)
            {
                var matchInfo = gameManager.GetMatchInfo();
                gameTime = matchInfo.time;
            }
            
            return (alivePlayers, status, gameTime);
        }
    }
} 