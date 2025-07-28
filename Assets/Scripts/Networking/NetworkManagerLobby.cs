using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

namespace WorldOfBalance.Networking
{
    /// <summary>
    /// NetworkManager для PvP-игры "Мир Баланса"
    /// Управляет подключением игроков и загрузкой сцен
    /// </summary>
    public class NetworkManagerLobby : NetworkManager
    {
        [Header("Game Settings")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform spawnPointA;
        [SerializeField] private Transform spawnPointB;
        
        [Header("UI References")]
        [SerializeField] private GameObject connectionMenu;
        [SerializeField] private GameObject gameHUD;
        
        private bool isHost = false;
        
        public override void Start()
        {
            base.Start();
            ShowConnectionMenu();
        }
        
        /// <summary>
        /// Показать меню подключения
        /// </summary>
        public void ShowConnectionMenu()
        {
            if (connectionMenu != null)
                connectionMenu.SetActive(true);
            if (gameHUD != null)
                gameHUD.SetActive(false);
        }
        
        /// <summary>
        /// Показать игровой HUD
        /// </summary>
        public void ShowGameHUD()
        {
            if (connectionMenu != null)
                connectionMenu.SetActive(false);
            if (gameHUD != null)
                gameHUD.SetActive(true);
        }
        
        /// <summary>
        /// Создать хост (сервер)
        /// </summary>
        public void CreateHost()
        {
            isHost = true;
            StartHost();
            ShowGameHUD();
        }
        
        /// <summary>
        /// Подключиться к серверу
        /// </summary>
        public void JoinGame()
        {
            isHost = false;
            StartClient();
            ShowGameHUD();
        }
        
        /// <summary>
        /// Отключиться от игры
        /// </summary>
        public void Disconnect()
        {
            if (NetworkServer.active && NetworkClient.active)
            {
                StopHost();
            }
            else if (NetworkClient.active)
            {
                StopClient();
            }
            else if (NetworkServer.active)
            {
                StopServer();
            }
            
            ShowConnectionMenu();
        }
        
        /// <summary>
        /// Вызывается когда клиент подключается к серверу
        /// </summary>
        public override void OnClientConnect()
        {
            base.OnClientConnect();
            Debug.Log("Клиент подключился к серверу");
        }
        
        /// <summary>
        /// Вызывается когда клиент отключается от сервера
        /// </summary>
        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            Debug.Log("Клиент отключился от сервера");
            ShowConnectionMenu();
        }
        
        /// <summary>
        /// Вызывается когда сервер запускается
        /// </summary>
        public override void OnStartServer()
        {
            base.OnStartServer();
            Debug.Log("Сервер запущен");
        }
        
        /// <summary>
        /// Вызывается когда игрок подключается к серверу
        /// </summary>
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            // Определяем позицию спавна в зависимости от количества игроков
            Transform spawnPoint = numPlayers == 0 ? spawnPointA : spawnPointB;
            
            // Создаем игрока
            GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            NetworkServer.AddPlayerForConnection(conn, player);
            
            Debug.Log($"Игрок {conn.connectionId} подключился и создан в позиции {spawnPoint.name}");
        }
        
        /// <summary>
        /// Вызывается когда игрок отключается от сервера
        /// </summary>
        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);
            Debug.Log($"Игрок {conn.connectionId} отключился");
        }
    }
} 