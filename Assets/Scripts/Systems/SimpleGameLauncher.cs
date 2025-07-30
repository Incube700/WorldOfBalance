using UnityEngine;

public class SimpleGameLauncher : MonoBehaviour
{
    void Start()
    {
        // Ждем один кадр, чтобы все компоненты инициализировались
        Invoke(nameof(StartGame), 0.1f);
    }
    
    void StartGame()
    {
        // Включаем все игровые компоненты
        EnableAllGameComponents();
        
        // Запускаем время
        Time.timeScale = 1f;
        
        Debug.Log("Игра запущена!");
    }
    
    void EnableAllGameComponents()
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
        
        Debug.Log("Все игровые компоненты включены!");
    }
} 