using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameUI;
    
    [Header("Game Manager")]
    [SerializeField] private LocalGameManager gameManager;
    
    void Start()
    {
        // Находим LocalGameManager если не назначен
        if (gameManager == null)
            gameManager = FindObjectOfType<LocalGameManager>();
            
        // Показываем главное меню при старте
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
            
        if (gameUI != null)
            gameUI.SetActive(false);
            
        // Останавливаем время
        Time.timeScale = 0f;
    }
    
    public void StartGame()
    {
        // Скрываем главное меню
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
            
        // Показываем игровой UI
        if (gameUI != null)
            gameUI.SetActive(true);
            
        // Запускаем игру через LocalGameManager
        if (gameManager != null)
        {
            gameManager.StartGame();
        }
        else
        {
            // Fallback - просто запускаем время
            Time.timeScale = 1f;
        }
        
        Debug.Log("Игра запущена!");
    }
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
} 