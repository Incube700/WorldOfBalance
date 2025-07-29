using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WorldOfBalance.Networking;

namespace WorldOfBalance.UI
{
    public class ConnectionMenu : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Button hostButton;
        [SerializeField] private Button joinButton;
        [SerializeField] private Button disconnectButton;
        [SerializeField] private TMP_InputField addressInput;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private GameObject loadingIndicator;
        
        [Header("Components")]
        [SerializeField] private NetworkManagerLobby networkManager;
        
        private void Start()
        {
            // Получаем NetworkManager с типизацией
            if (networkManager == null)
                networkManager = FindObjectOfType<NetworkManagerLobby>();
            
            if (networkManager == null)
            {
                Debug.LogError("ConnectionMenu: NetworkManagerLobby not found!");
                return;
            }
            
            SetupUI();
            UpdateUIState();
        }
        
        /// <summary>
        /// Настраивает UI элементы
        /// </summary>
        private void SetupUI()
        {
            if (hostButton != null)
                hostButton.onClick.AddListener(OnHostButtonClicked);
            
            if (joinButton != null)
                joinButton.onClick.AddListener(OnJoinButtonClicked);
            
            if (disconnectButton != null)
                disconnectButton.onClick.AddListener(OnDisconnectButtonClicked);
            
            if (addressInput != null)
                addressInput.text = "localhost";
        }
        
        /// <summary>
        /// Обновляет состояние UI в зависимости от подключения
        /// </summary>
        private void UpdateUIState()
        {
            bool isConnected = NetworkClient.active || NetworkServer.active;
            
            if (hostButton != null)
                hostButton.interactable = !isConnected;
            
            if (joinButton != null)
                joinButton.interactable = !isConnected;
            
            if (disconnectButton != null)
                disconnectButton.interactable = isConnected;
            
            if (addressInput != null)
                addressInput.interactable = !isConnected;
        }
        
        /// <summary>
        /// Обработчик нажатия кнопки "Host"
        /// </summary>
        public void OnHostButtonClicked()
        {
            if (networkManager == null) return;
            
            SetLoading(true);
            UpdateStatus("Создание сервера...");
            
            try
            {
                networkManager.CreateHost();
                UpdateStatus("Сервер создан успешно!");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Ошибка создания сервера: {e.Message}");
                UpdateStatus("Ошибка создания сервера!");
                SetLoading(false);
            }
        }
        
        /// <summary>
        /// Обработчик нажатия кнопки "Join"
        /// </summary>
        public void OnJoinButtonClicked()
        {
            if (networkManager == null) return;
            
            string address = addressInput != null ? addressInput.text : "localhost";
            
            SetLoading(true);
            UpdateStatus($"Подключение к {address}...");
            
            try
            {
                networkManager.JoinGame(address);
                UpdateStatus("Подключение успешно!");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Ошибка подключения: {e.Message}");
                UpdateStatus("Ошибка подключения!");
                SetLoading(false);
            }
        }
        
        /// <summary>
        /// Обработчик нажатия кнопки "Disconnect"
        /// </summary>
        public void OnDisconnectButtonClicked()
        {
            if (networkManager == null) return;
            
            networkManager.Disconnect();
            UpdateStatus("Отключено");
            SetLoading(false);
        }
        
        /// <summary>
        /// Обновляет текст статуса
        /// </summary>
        /// <param name="status">Новый статус</param>
        private void UpdateStatus(string status)
        {
            if (statusText != null)
            {
                statusText.text = status;
            }
            
            Debug.Log($"ConnectionMenu Status: {status}");
        }
        
        /// <summary>
        /// Устанавливает состояние загрузки
        /// </summary>
        /// <param name="loading">Состояние загрузки</param>
        private void SetLoading(bool loading)
        {
            if (loadingIndicator != null)
            {
                loadingIndicator.SetActive(loading);
            }
            
            UpdateUIState();
        }
        
        /// <summary>
        /// Обработчик изменения состояния сети
        /// </summary>
        private void OnNetworkStateChanged()
        {
            UpdateUIState();
            
            if (NetworkClient.active || NetworkServer.active)
            {
                UpdateStatus("Подключено");
                SetLoading(false);
            }
            else
            {
                UpdateStatus("Отключено");
                SetLoading(false);
            }
        }
        
        private void Update()
        {
            // Проверяем состояние сети
            if (NetworkClient.active || NetworkServer.active)
            {
                if (statusText != null && statusText.text != "Подключено")
                {
                    OnNetworkStateChanged();
                }
            }
        }
        
        /// <summary>
        /// Получает информацию о состоянии подключения
        /// </summary>
        /// <returns>Кортеж с информацией о подключении</returns>
        public (bool isConnected, bool isHost, bool isClient) GetConnectionInfo()
        {
            bool isConnected = NetworkClient.active || NetworkServer.active;
            bool isHost = NetworkServer.active && NetworkClient.active;
            bool isClient = NetworkClient.active && !NetworkServer.active;
            
            return (isConnected, isHost, isClient);
        }
    }
} 