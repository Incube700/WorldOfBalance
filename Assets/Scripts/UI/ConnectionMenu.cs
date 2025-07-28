using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WorldOfBalance.Networking;

namespace WorldOfBalance.UI
{
    /// <summary>
    /// UI меню для подключения к PvP-игре "Мир Баланса"
    /// </summary>
    public class ConnectionMenu : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button hostButton;
        [SerializeField] private Button joinButton;
        [SerializeField] private Button disconnectButton;
        [SerializeField] private TMP_InputField ipInputField;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private GameObject loadingPanel;
        
        [Header("Settings")]
        [SerializeField] private string defaultIP = "localhost";
        [SerializeField] private int defaultPort = 7777;
        
        private NetworkManagerLobby networkManager;
        
        private void Start()
        {
            networkManager = FindObjectOfType<NetworkManagerLobby>();
            if (networkManager == null)
            {
                Debug.LogError("NetworkManagerLobby не найден на сцене!");
                return;
            }
            
            // Настройка кнопок
            if (hostButton != null)
                hostButton.onClick.AddListener(OnHostButtonClicked);
                
            if (joinButton != null)
                joinButton.onClick.AddListener(OnJoinButtonClicked);
                
            if (disconnectButton != null)
                disconnectButton.onClick.AddListener(OnDisconnectButtonClicked);
            
            // Настройка поля IP
            if (ipInputField != null)
            {
                ipInputField.text = defaultIP;
                networkManager.networkAddress = defaultIP;
            }
            
            // Настройка порта
            networkManager.networkPort = defaultPort;
            
            UpdateUI();
        }
        
        /// <summary>
        /// Обработка нажатия кнопки "Создать хост"
        /// </summary>
        private void OnHostButtonClicked()
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
        /// Обработка нажатия кнопки "Подключиться"
        /// </summary>
        private void OnJoinButtonClicked()
        {
            if (networkManager == null) return;
            
            // Обновляем IP адрес
            if (ipInputField != null && !string.IsNullOrEmpty(ipInputField.text))
            {
                networkManager.networkAddress = ipInputField.text;
            }
            
            SetLoading(true);
            UpdateStatus("Подключение к серверу...");
            
            try
            {
                networkManager.JoinGame();
                UpdateStatus("Подключение к серверу...");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Ошибка подключения: {e.Message}");
                UpdateStatus("Ошибка подключения!");
                SetLoading(false);
            }
        }
        
        /// <summary>
        /// Обработка нажатия кнопки "Отключиться"
        /// </summary>
        private void OnDisconnectButtonClicked()
        {
            if (networkManager == null) return;
            
            networkManager.Disconnect();
            UpdateStatus("Отключено от сервера");
            SetLoading(false);
        }
        
        /// <summary>
        /// Обновление UI в зависимости от состояния подключения
        /// </summary>
        private void UpdateUI()
        {
            bool isConnected = NetworkClient.active || NetworkServer.active;
            
            if (hostButton != null)
                hostButton.interactable = !isConnected;
                
            if (joinButton != null)
                joinButton.interactable = !isConnected;
                
            if (disconnectButton != null)
                disconnectButton.gameObject.SetActive(isConnected);
                
            if (ipInputField != null)
                ipInputField.interactable = !isConnected;
        }
        
        /// <summary>
        /// Установка состояния загрузки
        /// </summary>
        private void SetLoading(bool isLoading)
        {
            if (loadingPanel != null)
                loadingPanel.SetActive(isLoading);
        }
        
        /// <summary>
        /// Обновление текста статуса
        /// </summary>
        private void UpdateStatus(string status)
        {
            if (statusText != null)
                statusText.text = status;
        }
        
        /// <summary>
        /// Обновление UI при изменении состояния сети
        /// </summary>
        private void Update()
        {
            UpdateUI();
            
            // Обновление статуса в зависимости от состояния сети
            if (NetworkServer.active && NetworkClient.active)
            {
                UpdateStatus("Хост активен");
                SetLoading(false);
            }
            else if (NetworkClient.active)
            {
                UpdateStatus("Подключен к серверу");
                SetLoading(false);
            }
            else if (NetworkServer.active)
            {
                UpdateStatus("Сервер активен");
                SetLoading(false);
            }
            else
            {
                UpdateStatus("Не подключен");
                SetLoading(false);
            }
        }
        
        /// <summary>
        /// Обработка изменения IP адреса
        /// </summary>
        public void OnIPAddressChanged(string newIP)
        {
            if (networkManager != null && !string.IsNullOrEmpty(newIP))
            {
                networkManager.networkAddress = newIP;
            }
        }
    }
} 