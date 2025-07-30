using UnityEngine;
using Mirror;

public class NetworkCleaner : MonoBehaviour
{
    void Awake()
    {
        // Останавливаем все сетевые процессы
        if (NetworkServer.active)
        {
            NetworkServer.Shutdown();
        }
        
        if (NetworkClient.active)
        {
            NetworkClient.Shutdown();
        }
        
        // Удаляем только NetworkManager, НЕ игровые объекты
        NetworkManager[] networkManagers = FindObjectsOfType<NetworkManager>();
        foreach (NetworkManager manager in networkManagers)
        {
            if (manager != null && manager.gameObject.name.Contains("NetworkManager"))
            {
                DestroyImmediate(manager.gameObject);
                Debug.Log("NetworkManager удален!");
            }
        }
        
        // Отключаем все NetworkIdentity, НЕ удаляем объекты
        NetworkIdentity[] identities = FindObjectsOfType<NetworkIdentity>();
        foreach (NetworkIdentity identity in identities)
        {
            if (identity != null)
            {
                identity.enabled = false;
                Debug.Log($"NetworkIdentity отключен на {identity.gameObject.name}");
            }
        }
        
        // Отключаем все NetworkTransform, НЕ удаляем объекты
        var allComponents = FindObjectsOfType<MonoBehaviour>();
        foreach (var component in allComponents)
        {
            if (component != null && component.GetType().Name == "NetworkTransform")
            {
                component.enabled = false;
                Debug.Log($"NetworkTransform отключен на {component.gameObject.name}");
            }
        }
        
        Debug.Log("Все сетевые компоненты очищены!");
    }
    
    void Start()
    {
        // Убеждаемся, что время запущено
        Time.timeScale = 1f;
        
        // Включаем все игровые компоненты
        EnableGameComponents();
    }
    
    void EnableGameComponents()
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
        
        Debug.Log("Игровые компоненты включены!");
    }
} 