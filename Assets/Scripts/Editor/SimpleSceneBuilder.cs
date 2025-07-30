using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class SimpleSceneBuilder : MonoBehaviour
{
    [MenuItem("Tools/Build Simple Local Scene")]
    public static void BuildSimpleLocalScene()
    {
        Debug.Log("=== НАЧИНАЕМ СБОРКУ СЦЕНЫ ===");
        
        // 1. Очищаем от сетевых компонентов
        CleanNetworkStuff();
        
        // 2. Создаем GameManager
        CreateGameManager();
        
        // 3. Создаем простое меню
        CreateSimpleMenu();
        
        // 4. Проверяем объекты
        CheckObjects();
        
        Debug.Log("=== СБОРКА ЗАВЕРШЕНА ===");
        Debug.Log("Теперь нажмите Play и проверьте игру!");
    }
    
    static void CleanNetworkStuff()
    {
        Debug.Log("1. Очищаем сетевые компоненты...");
        
        // Удаляем все объекты с NetworkManager
        var allObjects = FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            if (obj.name.Contains("NetworkManager") || obj.name.Contains("Network"))
            {
                DestroyImmediate(obj);
                Debug.Log($"Удален: {obj.name}");
            }
        }
        
        // Отключаем NetworkIdentity на всех объектах (если есть)
        var identities = FindObjectsOfType<MonoBehaviour>();
        foreach (var comp in identities)
        {
            if (comp != null && comp.GetType().Name == "NetworkIdentity")
            {
                comp.enabled = false;
                Debug.Log($"NetworkIdentity отключен на {comp.gameObject.name}");
            }
        }
    }
    
    static void CreateGameManager()
    {
        Debug.Log("2. Создаем LocalGameManager...");
        
        // Удаляем старый если есть
        var oldManager = FindObjectOfType<LocalGameManager>();
        if (oldManager != null)
        {
            DestroyImmediate(oldManager.gameObject);
        }
        
        // Создаем новый
        var gameManagerObj = new GameObject("LocalGameManager");
        gameManagerObj.AddComponent<LocalGameManager>();
        
        Debug.Log("LocalGameManager создан!");
    }
    
    static void CreateSimpleMenu()
    {
        Debug.Log("3. Создаем простое меню...");
        
        // Удаляем старый Canvas
        var oldCanvas = FindObjectOfType<Canvas>();
        if (oldCanvas != null)
        {
            DestroyImmediate(oldCanvas.gameObject);
        }
        
        // Создаем Canvas
        var canvasObj = new GameObject("Canvas");
        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Создаем Panel
        var panelObj = new GameObject("MenuPanel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        
        var panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.9f);
        
        var panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        // Добавляем MainMenu скрипт
        var mainMenu = panelObj.AddComponent<MainMenu>();
        
        // Создаем кнопки
        CreateSimpleButton(panelObj, "StartButton", "СТАРТ ИГРЫ", new Vector2(0, 50), () => {
            Debug.Log("Кнопка СТАРТ нажата!");
            mainMenu.StartGame();
        });
        
        CreateSimpleButton(panelObj, "QuitButton", "ВЫХОД", new Vector2(0, -50), () => {
            Debug.Log("Кнопка ВЫХОД нажата!");
            mainMenu.QuitGame();
        });
        
        Debug.Log("Меню создано!");
    }
    
    static void CreateSimpleButton(GameObject parent, string name, string text, Vector2 position, System.Action onClick)
    {
        var buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent.transform, false);
        
        // Кнопка
        var buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        var button = buttonObj.AddComponent<Button>();
        button.onClick.AddListener(() => onClick?.Invoke());
        
        // Размер и позиция
        var buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.sizeDelta = new Vector2(250, 60);
        buttonRect.anchoredPosition = position;
        
        // Текст
        var textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        var buttonText = textObj.AddComponent<Text>();
        buttonText.text = text;
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 28;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;
        
        var textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        Debug.Log($"Кнопка {name} создана");
    }
    
    static void CheckObjects()
    {
        Debug.Log("4. Проверяем объекты...");
        
        // Проверяем Player
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Debug.Log($"✅ Player найден: {player.name}");
            
            // Проверяем компоненты
            if (player.GetComponent<TankController>() != null)
                Debug.Log("✅ TankController есть");
            else
                Debug.LogWarning("❌ TankController отсутствует");
                
            if (player.GetComponent<HealthSystem>() != null)
                Debug.Log("✅ HealthSystem есть");
            else
                Debug.LogWarning("❌ HealthSystem отсутствует");
        }
        else
        {
            Debug.LogError("❌ Player НЕ НАЙДЕН!");
        }
        
        // Проверяем Enemy
        var enemy = GameObject.Find("Enemy");
        if (enemy != null)
        {
            Debug.Log($"✅ Enemy найден: {enemy.name}");
            
            if (enemy.GetComponent<EnemyAIController>() != null)
                Debug.Log("✅ EnemyAIController есть");
            else
                Debug.LogWarning("❌ EnemyAIController отсутствует");
        }
        else
        {
            Debug.LogError("❌ Enemy НЕ НАЙДЕН!");
        }
        
        // Проверяем Projectile
        var projectile = GameObject.Find("Projectile");
        if (projectile != null)
        {
            Debug.Log($"✅ Projectile найден: {projectile.name}");
            
            if (projectile.GetComponent<Projectile>() != null)
                Debug.Log("✅ Projectile компонент есть");
            else
                Debug.LogWarning("❌ Projectile компонент отсутствует");
        }
        else
        {
            Debug.LogError("❌ Projectile НЕ НАЙДЕН!");
        }
        
        Debug.Log("Проверка завершена!");
    }
} 