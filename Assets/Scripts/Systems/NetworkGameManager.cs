using UnityEngine;
using Mirror;
using WorldOfBalance.Player;
using WorldOfBalance.Map;
using WorldOfBalance.UI;

namespace WorldOfBalance.Systems
{
    /// <summary>
    /// Сетевой менеджер игры для PvP-игры "Мир Баланса"
    /// Управляет состоянием игры, победой и синхронизацией
    /// </summary>
    public class NetworkGameManager : NetworkBehaviour
    {
        [Header("Game Settings")]
        [SerializeField] private float matchDuration = 300f; // 5 минут
        [SerializeField] private int maxScore = 10;
        [SerializeField] private float respawnDelay = 3f;
        
        [Header("UI References")]
        [SerializeField] private GameObject gameUI;
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject gameOverMenu;
        [SerializeField] private GameHUD gameHUD;
        
        [Header("Managers")]
        [SerializeField] private SpawnManager spawnManager;
        
        // Сетевые переменные
        [SyncVar] private float currentMatchTime;
        [SyncVar] private bool isGamePaused = false;
        [SyncVar] private bool isGameOver = false;
        [SyncVar] private string gameOverReason = "";
        [SyncVar] private int player1Score = 0;
        [SyncVar] private int player2Score = 0;
        
        // Локальные переменные
        private NetworkPlayerController localPlayer;
        private bool isLocalGamePaused = false;
        
        // События
        public System.Action<string> OnGameOver;
        public System.Action OnPlayerRespawned;
        
        private void Start()
        {
            if (isServer)
            {
                InitializeGame();
            }
        }
        
        private void Update()
        {
            if (isServer)
            {
                UpdateMatchTimer();
                CheckGameOverConditions();
            }
            
            if (isLocalPlayer)
            {
                HandleInput();
            }
        }
        
        /// <summary>
        /// Инициализация игры на сервере
        /// </summary>
        [Server]
        private void InitializeGame()
        {
            currentMatchTime = matchDuration;
            isGamePaused = false;
            isGameOver = false;
            gameOverReason = "";
            player1Score = 0;
            player2Score = 0;
            
            // Создаем арену
            if (spawnManager != null)
            {
                spawnManager.CreateArena();
            }
            
            Debug.Log("Сетевая игра инициализирована!");
        }
        
        /// <summary>
        /// Обновление таймера матча на сервере
        /// </summary>
        [Server]
        private void UpdateMatchTimer()
        {
            if (isGamePaused || isGameOver) return;
            
            currentMatchTime -= Time.deltaTime;
            
            if (currentMatchTime <= 0)
            {
                EndMatch("Время вышло!");
            }
        }
        
        /// <summary>
        /// Проверка условий окончания игры на сервере
        /// </summary>
        [Server]
        private void CheckGameOverConditions()
        {
            if (isGameOver) return;
            
            // Проверяем количество живых игроков
            NetworkPlayerController[] players = FindObjectsOfType<NetworkPlayerController>();
            int alivePlayers = 0;
            NetworkPlayerController lastAlivePlayer = null;
            
            foreach (var player in players)
            {
                if (!player.IsDead)
                {
                    alivePlayers++;
                    lastAlivePlayer = player;
                }
            }
            
            // Определяем победителя
            if (alivePlayers == 0)
            {
                EndMatch("Ничья - оба игрока погибли!");
            }
            else if (alivePlayers == 1)
            {
                string winnerName = lastAlivePlayer.IsLocalPlayer ? "Игрок 1" : "Игрок 2";
                EndMatch($"{winnerName} победил!");
                
                // Увеличиваем счет победителю
                if (lastAlivePlayer.IsLocalPlayer)
                {
                    player1Score++;
                }
                else
                {
                    player2Score++;
                }
            }
        }
        
        /// <summary>
        /// Обработка ввода локального игрока
        /// </summary>
        private void HandleInput()
        {
            // Пауза
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CmdTogglePause();
            }
            
            // Рестарт (только для хоста)
            if (Input.GetKeyDown(KeyCode.R) && isServer)
            {
                RestartGame();
            }
        }
        
        /// <summary>
        /// Команда переключения паузы
        /// </summary>
        [Command(requiresAuthority = false)]
        private void CmdTogglePause()
        {
            if (!isServer) return;
            
            isGamePaused = !isGamePaused;
            
            if (isGamePaused)
            {
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }
            
            RpcUpdatePauseState(isGamePaused);
        }
        
        /// <summary>
        /// Обновление состояния паузы на клиентах
        /// </summary>
        [ClientRpc]
        private void RpcUpdatePauseState(bool paused)
        {
            isLocalGamePaused = paused;
            
            if (pauseMenu != null)
            {
                pauseMenu.SetActive(paused);
            }
        }
        
        /// <summary>
        /// Завершение матча на сервере
        /// </summary>
        [Server]
        private void EndMatch(string reason)
        {
            isGameOver = true;
            gameOverReason = reason;
            
            Debug.Log($"Матч завершен: {reason}");
            
            // Уведомляем клиентов
            RpcEndMatch(reason);
        }
        
        /// <summary>
        /// Уведомление клиентов о завершении матча
        /// </summary>
        [ClientRpc]
        private void RpcEndMatch(string reason)
        {
            isGameOver = true;
            gameOverReason = reason;
            
            if (gameOverMenu != null)
            {
                gameOverMenu.SetActive(true);
            }
            
            OnGameOver?.Invoke(reason);
            
            Debug.Log($"Игра окончена: {reason}");
        }
        
        /// <summary>
        /// Перезапуск игры (только для сервера)
        /// </summary>
        [Server]
        public void RestartGame()
        {
            Time.timeScale = 1f;
            
            // Сбрасываем состояние игры
            currentMatchTime = matchDuration;
            isGamePaused = false;
            isGameOver = false;
            gameOverReason = "";
            player1Score = 0;
            player2Score = 0;
            
            // Очищаем арену и создаем заново
            if (spawnManager != null)
            {
                spawnManager.ClearArena();
                spawnManager.CreateArena();
            }
            
            // Уведомляем клиентов
            RpcRestartGame();
            
            Debug.Log("Игра перезапущена!");
        }
        
        /// <summary>
        /// Уведомление клиентов о перезапуске игры
        /// </summary>
        [ClientRpc]
        private void RpcRestartGame()
        {
            Time.timeScale = 1f;
            
            if (gameOverMenu != null)
            {
                gameOverMenu.SetActive(false);
            }
            
            if (pauseMenu != null)
            {
                pauseMenu.SetActive(false);
            }
            
            Debug.Log("Игра перезапущена!");
        }
        
        /// <summary>
        /// Обработка смерти игрока
        /// </summary>
        [Server]
        public void OnPlayerDeath(GameObject deadPlayer)
        {
            Debug.Log($"Игрок {deadPlayer.name} погиб!");
            
            // Респавним игрока через некоторое время
            StartCoroutine(RespawnPlayerWithDelay(deadPlayer, respawnDelay));
        }
        
        /// <summary>
        /// Корутина респауна игрока с задержкой
        /// </summary>
        private System.Collections.IEnumerator RespawnPlayerWithDelay(GameObject player, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            if (spawnManager != null && player != null)
            {
                spawnManager.RespawnPlayer(player);
                OnPlayerRespawned?.Invoke();
            }
        }
        
        /// <summary>
        /// Получение оставшегося времени матча
        /// </summary>
        public float GetMatchTime()
        {
            return currentMatchTime;
        }
        
        /// <summary>
        /// Получение форматированного времени
        /// </summary>
        public string GetFormattedTime()
        {
            int minutes = Mathf.FloorToInt(currentMatchTime / 60f);
            int seconds = Mathf.FloorToInt(currentMatchTime % 60f);
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        
        /// <summary>
        /// Получение счета игроков
        /// </summary>
        public (int player1Score, int player2Score) GetScores()
        {
            return (player1Score, player2Score);
        }
        
        /// <summary>
        /// Проверка, пауза ли игра
        /// </summary>
        public bool IsGamePaused()
        {
            return isGamePaused || isLocalGamePaused;
        }
        
        /// <summary>
        /// Проверка, окончена ли игра
        /// </summary>
        public bool IsGameOver()
        {
            return isGameOver;
        }
        
        /// <summary>
        /// Получение причины окончания игры
        /// </summary>
        public string GetGameOverReason()
        {
            return gameOverReason;
        }
        
        /// <summary>
        /// Выход в главное меню
        /// </summary>
        public void QuitToMenu()
        {
            Time.timeScale = 1f;
            
            // Отключаемся от сети
            if (NetworkClient.active)
            {
                NetworkManager.singleton.StopClient();
            }
            else if (NetworkServer.active)
            {
                NetworkManager.singleton.StopHost();
            }
        }
        
        /// <summary>
        /// Выход из игры
        /// </summary>
        public void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        
        /// <summary>
        /// Визуализация в редакторе
        /// </summary>
        private void OnGUI()
        {
            if (isLocalPlayer)
            {
                GUI.Label(new Rect(10, 90, 200, 20), $"Match Time: {GetFormattedTime()}");
                GUI.Label(new Rect(10, 110, 200, 20), $"Score: {player1Score} - {player2Score}");
                GUI.Label(new Rect(10, 130, 200, 20), $"Game Over: {isGameOver}");
                GUI.Label(new Rect(10, 150, 200, 20), $"Paused: {IsGamePaused()}");
            }
        }
    }
} 