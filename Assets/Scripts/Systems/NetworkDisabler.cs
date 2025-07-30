using UnityEngine;
using Mirror;

public class NetworkDisabler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool disableNetworkComponents = true;
    [SerializeField] private bool autoStartNetwork = true;
    
    void Start()
    {
        if (disableNetworkComponents)
        {
            DisableNetworkComponents();
        }
        
        if (autoStartNetwork)
        {
            StartNetworkIfNeeded();
        }
    }
    
    void DisableNetworkComponents()
    {
        // Отключаем все NetworkIdentity компоненты
        NetworkIdentity[] networkIdentities = FindObjectsOfType<NetworkIdentity>();
        foreach (NetworkIdentity identity in networkIdentities)
        {
            if (identity != null)
            {
                identity.enabled = false;
                Debug.Log($"Отключен NetworkIdentity на {identity.gameObject.name}");
            }
        }
        
        // Отключаем все NetworkTransform компоненты (если есть)
        var networkTransforms = FindObjectsOfType<MonoBehaviour>();
        foreach (var component in networkTransforms)
        {
            if (component != null && component.GetType().Name == "NetworkTransform")
            {
                component.enabled = false;
                Debug.Log($"Отключен NetworkTransform на {component.gameObject.name}");
            }
        }
        
        // Отключаем все NetworkBehaviour компоненты (кроме Mirror)
        MonoBehaviour[] allBehaviours = FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour behaviour in allBehaviours)
        {
            if (behaviour != null && behaviour.GetType().IsSubclassOf(typeof(NetworkBehaviour)))
            {
                // Проверяем, что это не Mirror компонент
                if (!behaviour.GetType().Namespace.StartsWith("Mirror"))
                {
                    behaviour.enabled = false;
                    Debug.Log($"Отключен NetworkBehaviour {behaviour.GetType().Name} на {behaviour.gameObject.name}");
                }
            }
        }
        
        Debug.Log("Все сетевые компоненты отключены!");
    }
    
    void StartNetworkIfNeeded()
    {
        // Проверяем, есть ли NetworkManager
        NetworkManager networkManager = FindObjectOfType<NetworkManager>();
        if (networkManager == null)
        {
            Debug.Log("NetworkManager не найден, создаем SimpleNetworkManager...");
            
            // Создаем NetworkManager
            GameObject networkManagerObj = new GameObject("NetworkManager");
            SimpleNetworkManager simpleNetworkManager = networkManagerObj.AddComponent<SimpleNetworkManager>();
            
            // Настраиваем базовые параметры
            simpleNetworkManager.playerPrefab = null; // Будем создавать игроков вручную
            
            Debug.Log("SimpleNetworkManager создан!");
        }
        else
        {
            Debug.Log("NetworkManager уже существует!");
        }
    }
} 