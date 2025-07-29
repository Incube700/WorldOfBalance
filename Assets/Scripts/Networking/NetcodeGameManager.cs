using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace WorldOfBalance.Networking
{
    /// <summary>
    /// NetworkManager для PvP-игры "Мир Баланса" с использованием Unity Netcode for GameObjects
    /// Управляет подключением игроков и загрузкой сцен
    /// </summary>
    public class NetcodeGameManager : NetworkBehaviour
    {
        [Header("Game Settings")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform spawnPointA;
        [SerializeField] private Transform spawnPointB;
        
        [Header("UI References")]
        [SerializeField] private GameObject connectionMenu;
        [SerializeField] private GameObject gameHUD;
        
        private NetworkManager networkManager;
        
        private void Start()
        {
            networkManager = NetworkManager.Singleton;
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
            networkManager.StartHost();
            ShowGameHUD();
        }
        
        /// <summary>
        /// Подключиться к серверу
        /// </summary>
        public void JoinGame()
        {
            networkManager.StartClient();
            ShowGameHUD();
        }
        
        /// <summary>
        /// Отключиться от игры
        /// </summary>
        public void Disconnect()
        {
            if (networkManager.IsHost)
            {
                networkManager.Shutdown();
            }
            else if (networkManager.IsClient)
            {
                networkManager.Shutdown();
            }
            
            ShowConnectionMenu();
        }
        
        /// <summary>
        /// Вызывается когда клиент подключается к серверу
        /// </summary>
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Debug.Log("Клиент подключился к серверу");
        }
        
        /// <summary>
        /// Вызывается когда клиент отключается от сервера
        /// </summary>
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            Debug.Log("Клиент отключился от сервера");
            ShowConnectionMenu();
        }
        
        /// <summary>
        /// Вызывается когда сервер запускается
        /// </summary>
        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                base.OnNetworkSpawn();
                Debug.Log("Сервер запущен");
            }
        }
        
        /// <summary>
        /// Создание игрока при подключении
        /// </summary>
        public void SpawnPlayer(ulong clientId)
        {
            if (!IsServer) return;
            
            // Определяем позицию спавна в зависимости от количества игроков
            Transform spawnPoint = NetworkManager.Singleton.ConnectedClientsIds.Count == 1 ? spawnPointA : spawnPointB;
            
            // Создаем игрока
            GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            NetworkObject networkObject = player.GetComponent<NetworkObject>();
            networkObject.SpawnAsPlayerObject(clientId);
            
            Debug.Log($"Игрок {clientId} подключился и создан в позиции {spawnPoint.name}");
        }
    }
}