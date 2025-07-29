using UnityEngine;
using Mirror;
using System;
using WorldOfBalance.Player;

namespace WorldOfBalance.Systems
{
    public class NetworkGameManager : NetworkBehaviour
    {
        [Header("Game Settings")]
        [SerializeField] private float matchDuration = 300f; // 5 минут
        [SerializeField] private bool isGamePaused = false;
        [SerializeField] private bool isGameOver = false;
        
        [Header("Scores")]
        [SerializeField] private int player1Score = 0;
        [SerializeField] private int player2Score = 0;
        
        // Network synchronization
        [SyncVar] private float networkMatchTime;
        [SyncVar] private bool networkIsGamePaused;
        [SyncVar] private bool networkIsGameOver;
        [SyncVar] private string gameOverReason = "";
        
        // Events
        public event Action<string> OnGameOver;
        public event Action OnGameRestarted;
        public event Action<bool> OnPauseStateChanged;
        
        // Properties
        public float MatchTime => networkMatchTime;
        public bool IsGamePaused => networkIsGamePaused;
        public bool IsGameOver => networkIsGameOver;
        public string GameOverReason => gameOverReason;
        public int Player1Score => player1Score;
        public int Player2Score => player2Score;
        
        private void Start()
        {
            if (isServer)
            {
                InitializeGame();
            }
        }
        
        private void Update()
        {
            if (!isServer) return;
            
            if (!isGamePaused && !isGameOver)
            {
                UpdateMatchTimer();
            }
        }
        
        /// <summary>
        /// Инициализирует игру на сервере
        /// </summary>
        [Server]
        private void InitializeGame()
        {
            networkMatchTime = matchDuration;
            networkIsGamePaused = false;
            networkIsGameOver = false;
            gameOverReason = "";
            
            player1Score = 0;
            player2Score = 0;
            
            Debug.Log("Game initialized!");
        }
        
        /// <summary>
        /// Обновляет таймер матча
        /// </summary>
        [Server]
        private void UpdateMatchTimer()
        {
            networkMatchTime -= Time.deltaTime;
            
            if (networkMatchTime <= 0)
            {
                EndMatch("Время вышло!");
            }
            else
            {
                CheckGameOverConditions();
            }
        }
        
        /// <summary>
        /// Проверяет условия окончания игры
        /// </summary>
        [Server]
        private void CheckGameOverConditions()
        {
            if (isGameOver) return;
            
            // Находим всех игроков с типизацией
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
            
            if (alivePlayers == 0)
            {
                EndMatch("Ничья - оба игрока погибли!");
            }
            else if (alivePlayers == 1)
            {
                string winnerName = lastAlivePlayer.IsLocalPlayer ? "Игрок 1" : "Игрок 2";
                EndMatch($"{winnerName} победил!");
                
                // Обновляем счет
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
        /// Завершает матч
        /// </summary>
        /// <param name="reason">Причина завершения</param>
        [Server]
        private void EndMatch(string reason)
        {
            if (isGameOver) return;
            
            isGameOver = true;
            networkIsGameOver = true;
            gameOverReason = reason;
            
            Debug.Log($"Game Over: {reason}");
            
            RpcEndMatch(reason);
        }
        
        /// <summary>
        /// Перезапускает игру
        /// </summary>
        [Server]
        public void RestartGame()
        {
            isGameOver = false;
            networkIsGameOver = false;
            gameOverReason = "";
            
            // Возрождаем всех игроков
            NetworkPlayerController[] players = FindObjectsOfType<NetworkPlayerController>();
            foreach (var player in players)
            {
                if (player != null)
                {
                    // Получаем HealthSystem с типизацией
                    HealthSystem healthSystem = player.GetComponent<HealthSystem>();
                    if (healthSystem != null)
                    {
                        healthSystem.Respawn();
                    }
                }
            }
            
            InitializeGame();
            RpcRestartGame();
            
            Debug.Log("Game restarted!");
        }
        
        /// <summary>
        /// Переключает паузу
        /// </summary>
        [Command]
        public void CmdTogglePause()
        {
            isGamePaused = !isGamePaused;
            networkIsGamePaused = isGamePaused;
            
            RpcUpdatePauseState(isGamePaused);
            
            Debug.Log($"Game paused: {isGamePaused}");
        }
        
        /// <summary>
        /// Обрабатывает смерть игрока
        /// </summary>
        /// <param name="player">Умерший игрок</param>
        [Server]
        public void OnPlayerDeath(NetworkPlayerController player)
        {
            Debug.Log($"Player {player.name} died!");
            
            // Проверяем условия окончания игры
            CheckGameOverConditions();
        }
        
        /// <summary>
        /// Уведомляет клиентов о завершении матча
        /// </summary>
        /// <param name="reason">Причина завершения</param>
        [ClientRpc]
        private void RpcEndMatch(string reason)
        {
            OnGameOver?.Invoke(reason);
            Debug.Log($"Game Over on client: {reason}");
        }
        
        /// <summary>
        /// Уведомляет клиентов о перезапуске игры
        /// </summary>
        [ClientRpc]
        private void RpcRestartGame()
        {
            OnGameRestarted?.Invoke();
            Debug.Log("Game restarted on client!");
        }
        
        /// <summary>
        /// Уведомляет клиентов об изменении состояния паузы
        /// </summary>
        /// <param name="paused">Состояние паузы</param>
        [ClientRpc]
        private void RpcUpdatePauseState(bool paused)
        {
            OnPauseStateChanged?.Invoke(paused);
            Debug.Log($"Pause state changed on client: {paused}");
        }
        
        /// <summary>
        /// Получает информацию о матче
        /// </summary>
        /// <returns>Кортеж с информацией о матче</returns>
        public (float time, bool paused, bool gameOver, string reason) GetMatchInfo()
        {
            return (networkMatchTime, networkIsGamePaused, networkIsGameOver, gameOverReason);
        }
        
        /// <summary>
        /// Получает информацию о счете
        /// </summary>
        /// <returns>Кортеж с счетом (player1, player2)</returns>
        public (int player1, int player2) GetScore()
        {
            return (player1Score, player2Score);
        }
    }
} 