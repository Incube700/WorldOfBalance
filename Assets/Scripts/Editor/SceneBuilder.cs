using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class SceneBuilder : MonoBehaviour
{
    [MenuItem("Tools/Build Local Scene")]
    public static void BuildLocalScene()
    {
        Debug.Log("Начинаем сборку локальной сцены...");
        
        // 1. Очищаем сцену от сетевых компонентов
        CleanNetworkComponents();
        
        // 2. Создаем LocalGameManager
        CreateLocalGameManager();
        
        // 3. Создаем UI
        CreateUI();
        
        // 4. Настраиваем объекты
        SetupObjects();
        
        Debug.Log("Сборка сцены завершена!");
    }
    
    static void CleanNetworkComponents()
    {
        Debug.Log("Очищаем сетевые компоненты...");
        
        // Удаляем NetworkManager
        NetworkManager[] networkManagers = FindObjectsOfType<NetworkManager>();
        foreach (NetworkManager manager in networkManagers)
        {
            if (manager != null)
            {
                DestroyImmediate(manager.gameObject);
                Debug.Log("NetworkManager удален");
            }
        }
        
        // Отключаем NetworkIdentity
        NetworkIdentity[] identities = FindObjectsOfType<NetworkIdentity>();
        foreach (NetworkIdentity identity in identities)
        {
            if (identity != null)
            {
                identity.enabled = false;
                Debug.Log($"NetworkIdentity отключен на {identity.gameObject.name}");
            }
        }
    }
    
    static void CreateLocalGameManager()
    {
        Debug.Log("Создаем LocalGameManager...");
        
        // Удаляем старый GameManager если есть
        LocalGameManager oldManager = FindObjectOfType<LocalGameManager>();
        if (oldManager != null)
        {
            DestroyImmediate(oldManager.gameObject);
        }
        
        // Создаем новый LocalGameManager
        GameObject gameManagerObj = new GameObject("LocalGameManager");
        LocalGameManager gameManager = gameManagerObj.AddComponent<LocalGameManager>();
        
        Debug.Log("LocalGameManager создан");
    }
    
    static void CreateUI()
    {
        Debug.Log("Создаем UI...");
        
        // Удаляем старый Canvas если есть
        Canvas oldCanvas = FindObjectOfType<Canvas>();
        if (oldCanvas != null)
        {
            DestroyImmediate(oldCanvas.gameObject);
        }
        
        // Создаем Canvas
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        // Добавляем CanvasScaler
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        // Добавляем GraphicRaycaster
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Создаем Panel для меню
        GameObject panelObj = new GameObject("MainMenuPanel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f);
        
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        // Добавляем MainMenu скрипт
        MainMenu mainMenu = panelObj.AddComponent<MainMenu>();
        
        // Создаем кнопки
        CreateButton(panelObj, "StartButton", "СТАРТ", new Vector2(0, 100), mainMenu.StartGame);
        CreateButton(panelObj, "QuitButton", "ВЫХОД", new Vector2(0, -100), mainMenu.QuitGame);
        
        // Настраиваем ссылки в LocalGameManager
        LocalGameManager gameManager = FindObjectOfType<LocalGameManager>();
        if (gameManager != null)
        {
            // Находим Player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                // Используем reflection для установки private поля
                var playerField = typeof(LocalGameManager).GetField("player", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (playerField != null)
                    playerField.SetValue(gameManager, player.GetComponent<TankController>());
            }
            
            // Используем reflection для установки mainMenu
            var mainMenuField = typeof(LocalGameManager).GetField("mainMenu", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (mainMenuField != null)
                mainMenuField.SetValue(gameManager, panelObj);
        }
        
        Debug.Log("UI создан");
    }
    
    static void CreateButton(GameObject parent, string name, string text, Vector2 position, System.Action onClick)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent.transform, false);
        
        // Добавляем компоненты кнопки
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        
        Button button = buttonObj.AddComponent<Button>();
        
        // Настраиваем RectTransform
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.sizeDelta = new Vector2(200, 50);
        buttonRect.anchoredPosition = position;
        
        // Создаем текст
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        Text buttonText = textObj.AddComponent<Text>();
        buttonText.text = text;
        buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        buttonText.fontSize = 24;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        // Настраиваем onClick
        button.onClick.AddListener(() => onClick?.Invoke());
        
        Debug.Log($"Кнопка {name} создана");
    }
    
    static void SetupObjects()
    {
        Debug.Log("Настраиваем объекты...");
        
        // Находим Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Убеждаемся, что у Player есть все необходимые компоненты
            if (player.GetComponent<TankController>() == null)
                player.AddComponent<TankController>();
                
            if (player.GetComponent<HealthSystem>() == null)
                player.AddComponent<HealthSystem>();
                
            if (player.GetComponent<ArmorSystem>() == null)
                player.AddComponent<ArmorSystem>();
                
            if (player.GetComponent<ProjectileSpawner>() == null)
                player.AddComponent<ProjectileSpawner>();
                
            Debug.Log("Player настроен");
        }
        
        // Находим Enemy
        GameObject enemy = GameObject.Find("Enemy");
        if (enemy != null)
        {
            // Убеждаемся, что у Enemy есть все необходимые компоненты
            if (enemy.GetComponent<EnemyAIController>() == null)
                enemy.AddComponent<EnemyAIController>();
                
            if (enemy.GetComponent<HealthSystem>() == null)
                enemy.AddComponent<HealthSystem>();
                
            if (enemy.GetComponent<ArmorSystem>() == null)
                enemy.AddComponent<ArmorSystem>();
                
            Debug.Log("Enemy настроен");
        }
        
        // Находим Projectile
        GameObject projectile = GameObject.Find("Projectile");
        if (projectile != null)
        {
            if (projectile.GetComponent<Projectile>() == null)
                projectile.AddComponent<Projectile>();
                
            Debug.Log("Projectile настроен");
        }
        
        Debug.Log("Объекты настроены");
    }
} 