using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class NetworkUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button disconnectButton;
    [SerializeField] private TMP_InputField addressInput;
    [SerializeField] private TextMeshProUGUI statusText;
    
    [Header("Network Manager")]
    [SerializeField] private GameNetworkManager networkManager;
    
    void Start()
    {
        // Setup button listeners
        if (hostButton != null)
        {
            hostButton.onClick.AddListener(OnHostButtonClicked);
        }
        
        if (clientButton != null)
        {
            clientButton.onClick.AddListener(OnClientButtonClicked);
        }
        
        if (disconnectButton != null)
        {
            disconnectButton.onClick.AddListener(OnDisconnectButtonClicked);
        }
        
        // Set default address
        if (addressInput != null)
        {
            addressInput.text = "localhost";
        }
        
        UpdateUI();
    }
    
    void Update()
    {
        UpdateStatusText();
    }
    
    void OnHostButtonClicked()
    {
        if (networkManager != null)
        {
            networkManager.StartHost();
            ShowGamePanel();
        }
    }
    
    void OnClientButtonClicked()
    {
        if (networkManager != null && addressInput != null)
        {
            networkManager.networkAddress = addressInput.text;
            networkManager.StartClient();
            ShowGamePanel();
        }
    }
    
    void OnDisconnectButtonClicked()
    {
        if (networkManager != null)
        {
            networkManager.StopNetwork();
            ShowMainMenu();
        }
    }
    
    void ShowMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (gamePanel != null) gamePanel.SetActive(false);
    }
    
    void ShowGamePanel()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(true);
    }
    
    void UpdateStatusText()
    {
        if (statusText == null) return;
        
        if (NetworkServer.active && NetworkClient.active)
        {
            statusText.text = "Host Mode";
        }
        else if (NetworkClient.active)
        {
            statusText.text = "Client Mode";
        }
        else if (NetworkServer.active)
        {
            statusText.text = "Server Mode";
        }
        else
        {
            statusText.text = "Disconnected";
        }
    }
    
    void UpdateUI()
    {
        bool isConnected = NetworkClient.active || NetworkServer.active;
        
        if (hostButton != null) hostButton.interactable = !isConnected;
        if (clientButton != null) clientButton.interactable = !isConnected;
        if (disconnectButton != null) disconnectButton.interactable = isConnected;
        if (addressInput != null) addressInput.interactable = !isConnected;
    }
    
    void OnDestroy()
    {
        // Clean up button listeners
        if (hostButton != null)
        {
            hostButton.onClick.RemoveListener(OnHostButtonClicked);
        }
        
        if (clientButton != null)
        {
            clientButton.onClick.RemoveListener(OnClientButtonClicked);
        }
        
        if (disconnectButton != null)
        {
            disconnectButton.onClick.RemoveListener(OnDisconnectButtonClicked);
        }
    }
} 