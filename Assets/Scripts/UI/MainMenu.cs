using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameUI;
    
    [Header("Game Starter")]
    [SerializeField] private GameStarter gameStarter;
    
    void Start()
    {
        // Находим GameStarter если не назначен
        if (gameStarter == null)
            gameStarter = FindObjectOfType<GameStarter>();
            
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
            
        // Запускаем игру через GameStarter
        if (gameStarter != null)
        {
            gameStarter.StartGame();
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