using UnityEngine;

public class ObjectPreserver : MonoBehaviour
{
    [Header("Important Objects")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject mainCamera;
    
    void Awake()
    {
        // Сохраняем ссылки на важные объекты
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
            
        if (enemy == null)
            enemy = GameObject.Find("Enemy");
            
        if (mainCamera == null)
            mainCamera = Camera.main?.gameObject;
            
        // Убеждаемся, что объекты не уничтожаются
        DontDestroyOnLoad(gameObject);
        
        if (player != null)
            DontDestroyOnLoad(player);
            
        if (enemy != null)
            DontDestroyOnLoad(enemy);
            
        if (mainCamera != null)
            DontDestroyOnLoad(mainCamera);
    }
    
    void Start()
    {
        // Проверяем, что все объекты на месте
        CheckObjects();
        
        // Включаем все компоненты
        EnableAllComponents();
    }
    
    void CheckObjects()
    {
        Debug.Log("=== ПРОВЕРКА ОБЪЕКТОВ ===");
        
        if (player != null)
            Debug.Log($"Player найден: {player.name}");
        else
            Debug.LogError("Player НЕ НАЙДЕН!");
            
        if (enemy != null)
            Debug.Log($"Enemy найден: {enemy.name}");
        else
            Debug.LogError("Enemy НЕ НАЙДЕН!");
            
        if (mainCamera != null)
            Debug.Log($"Camera найдена: {mainCamera.name}");
        else
            Debug.LogError("Camera НЕ НАЙДЕНА!");
            
        // Ищем все танки
        TankController[] tanks = FindObjectsOfType<TankController>();
        Debug.Log($"Найдено танков: {tanks.Length}");
        
        // Ищем всех врагов
        EnemyAIController[] enemies = FindObjectsOfType<EnemyAIController>();
        Debug.Log($"Найдено врагов: {enemies.Length}");
        
        Debug.Log("=== ПРОВЕРКА ЗАВЕРШЕНА ===");
    }
    
    void EnableAllComponents()
    {
        // Включаем все танки
        TankController[] tanks = FindObjectsOfType<TankController>();
        foreach (TankController tank in tanks)
        {
            if (tank != null)
            {
                tank.enabled = true;
                Debug.Log($"TankController включен на {tank.gameObject.name}");
            }
        }
        
        // Включаем ИИ врагов
        EnemyAIController[] enemies = FindObjectsOfType<EnemyAIController>();
        foreach (EnemyAIController enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.enabled = true;
                Debug.Log($"EnemyAIController включен на {enemy.gameObject.name}");
            }
        }
        
        // Включаем системы здоровья
        HealthSystem[] healthSystems = FindObjectsOfType<HealthSystem>();
        foreach (HealthSystem health in healthSystems)
        {
            if (health != null)
            {
                health.enabled = true;
                Debug.Log($"HealthSystem включен на {health.gameObject.name}");
            }
        }
        
        // Включаем системы брони
        ArmorSystem[] armorSystems = FindObjectsOfType<ArmorSystem>();
        foreach (ArmorSystem armor in armorSystems)
        {
            if (armor != null)
            {
                armor.enabled = true;
                Debug.Log($"ArmorSystem включен на {armor.gameObject.name}");
            }
        }
        
        // Включаем спавнеры снарядов
        ProjectileSpawner[] spawners = FindObjectsOfType<ProjectileSpawner>();
        foreach (ProjectileSpawner spawner in spawners)
        {
            if (spawner != null)
            {
                spawner.enabled = true;
                Debug.Log($"ProjectileSpawner включен на {spawner.gameObject.name}");
            }
        }
        
        Debug.Log("Все компоненты включены!");
    }
    
    void Update()
    {
        // Постоянно проверяем, что объекты на месте
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                Debug.Log("Player восстановлен!");
        }
        
        if (enemy == null)
        {
            enemy = GameObject.Find("Enemy");
            if (enemy != null)
                Debug.Log("Enemy восстановлен!");
        }
    }
} 