using UnityEngine;

public class GameStarter : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject projectile;
    
    [Header("UI")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameUI;
    
    void Start()
    {
        // Останавливаем время
        Time.timeScale = 0f;
        
        // Отключаем игровые объекты до старта
        DisableGameObjects();
        
        // Показываем главное меню
        if (mainMenu != null)
            mainMenu.SetActive(true);
            
        if (gameUI != null)
            gameUI.SetActive(false);
    }
    
    void DisableGameObjects()
    {
        // Отключаем все танки
        TankController[] tanks = FindObjectsOfType<TankController>();
        foreach (TankController tank in tanks)
        {
            if (tank != null)
                tank.enabled = false;
        }
        
        // Отключаем ИИ врагов
        EnemyAIController[] enemies = FindObjectsOfType<EnemyAIController>();
        foreach (EnemyAIController enemy in enemies)
        {
            if (enemy != null)
                enemy.enabled = false;
        }
        
        Debug.Log("Игровые объекты отключены до старта!");
    }
    
    public void StartGame()
    {
        // Включаем игровые объекты
        EnableGameObjects();
        
        // Скрываем главное меню
        if (mainMenu != null)
            mainMenu.SetActive(false);
            
        // Показываем игровой UI
        if (gameUI != null)
            gameUI.SetActive(true);
            
        // Запускаем время
        Time.timeScale = 1f;
        
        Debug.Log("Игра запущена!");
    }
    
    void EnableGameObjects()
    {
        // Включаем все танки
        TankController[] tanks = FindObjectsOfType<TankController>();
        foreach (TankController tank in tanks)
        {
            if (tank != null)
                tank.enabled = true;
        }
        
        // Включаем ИИ врагов
        EnemyAIController[] enemies = FindObjectsOfType<EnemyAIController>();
        foreach (EnemyAIController enemy in enemies)
        {
            if (enemy != null)
                enemy.enabled = true;
        }
        
        Debug.Log("Игровые объекты включены!");
    }
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
} 