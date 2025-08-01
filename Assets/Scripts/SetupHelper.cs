using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Помощник для автоматической настройки WorldOfBalance в Unity
/// Используйте в Editor для быстрой настройки сцены
/// </summary>
public class SetupHelper : MonoBehaviour
{
    [Header("Auto Setup - Click in Inspector")]
    [SerializeField] private bool setupComplete = false;
    
    [Header("Created Objects")]
    [SerializeField] private GameObject gameManagerObj;
    [SerializeField] private GameObject uiCanvasObj;
    [SerializeField] private GameObject hudObj;
    
    [ContextMenu("Setup Complete Game")]
    public void SetupCompleteGame()
    {
        Debug.Log("🎮 SetupHelper: Starting complete game setup...");
        
        // 1. Setup GameManager
        SetupGameManager();
        
        // 2. Setup UI Canvas and HUD
        SetupUISystem();
        
        // 3. Setup Camera
        SetupCamera();
        
        // 4. Find and setup tanks
        SetupTanks();
        
        // 5. Create arena if needed
        SetupArena();
        
        setupComplete = true;
        Debug.Log("✅ SetupHelper: Complete game setup finished!");
    }
    
    void SetupGameManager()
    {
        // Find existing GameManager
        SimpleGameManager existingGM = FindObjectOfType<SimpleGameManager>();
        
        if (existingGM == null)
        {
            // Create new GameManager
            gameManagerObj = new GameObject("SimpleGameManager");
            gameManagerObj.AddComponent<SimpleGameManager>();
            Debug.Log("✅ Created SimpleGameManager");
        }
        else
        {
            gameManagerObj = existingGM.gameObject;
            Debug.Log("✅ Found existing SimpleGameManager");
        }
    }
    
    void SetupUISystem()
    {
        // Find existing Canvas
        Canvas existingCanvas = FindObjectOfType<Canvas>();
        
        if (existingCanvas == null)
        {
            // Create UI Canvas
            uiCanvasObj = new GameObject("UI Canvas");
            Canvas canvas = uiCanvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler scaler = uiCanvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            uiCanvasObj.AddComponent<GraphicRaycaster>();
            
            Debug.Log("✅ Created UI Canvas");
        }
        else
        {
            uiCanvasObj = existingCanvas.gameObject;
            Debug.Log("✅ Found existing Canvas");
        }
        
        // Add SimpleHUD to Canvas
        SimpleHUD existingHUD = uiCanvasObj.GetComponent<SimpleHUD>();
        if (existingHUD == null)
        {
            uiCanvasObj.AddComponent<SimpleHUD>();
            Debug.Log("✅ Added SimpleHUD to Canvas");
        }
        
        // Create UI Panels
        CreateUIPanel("MainMenuPanel", "MAIN MENU", new Color(0.2f, 0.2f, 0.2f, 0.8f));
        CreateUIPanel("GameplayPanel", "GAMEPLAY HUD", new Color(0, 0, 0, 0));
        CreateUIPanel("GameOverPanel", "GAME OVER", new Color(0.8f, 0.2f, 0.2f, 0.8f));
        CreateUIPanel("VictoryPanel", "VICTORY!", new Color(0.2f, 0.8f, 0.2f, 0.8f));
        
        // Create HUD elements
        CreateHUDElements();
    }
    
    GameObject CreateUIPanel(string panelName, string titleText, Color backgroundColor)
    {
        // Check if panel already exists
        Transform existingPanel = uiCanvasObj.transform.Find(panelName);
        if (existingPanel != null)
        {
            Debug.Log($"✅ Found existing {panelName}");
            return existingPanel.gameObject;
        }
        
        // Create panel
        GameObject panel = new GameObject(panelName);
        panel.transform.SetParent(uiCanvasObj.transform, false);
        
        // Add RectTransform and Image
        RectTransform rectTransform = panel.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        
        Image image = panel.AddComponent<Image>();
        image.color = backgroundColor;
        
        // Add title text
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(panel.transform, false);
        
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.7f);
        titleRect.anchorMax = new Vector2(0.5f, 0.7f);
        titleRect.sizeDelta = new Vector2(400, 100);
        
        TextMeshProUGUI titleTextComp = titleObj.AddComponent<TextMeshProUGUI>();
        titleTextComp.text = titleText;
        titleTextComp.fontSize = 48;
        titleTextComp.color = Color.white;
        titleTextComp.alignment = TextAlignmentOptions.Center;
        
        // Create buttons for specific panels
        if (panelName == "MainMenuPanel")
        {
            CreateButton(panel, "PLAY", new Vector2(0.5f, 0.4f), "SimpleGameManager", "OnStartGameButton");
            CreateButton(panel, "QUIT", new Vector2(0.5f, 0.3f), "SimpleGameManager", "OnQuitButton");
        }
        else if (panelName == "GameOverPanel" || panelName == "VictoryPanel")
        {
            CreateButton(panel, "RESTART", new Vector2(0.5f, 0.4f), "SimpleGameManager", "OnRestartButton");
            CreateButton(panel, "MENU", new Vector2(0.5f, 0.3f), "SimpleGameManager", "OnMainMenuButton");
        }
        
        // Hide panels except MainMenu initially
        if (panelName != "MainMenuPanel")
        {
            panel.SetActive(false);
        }
        
        Debug.Log($"✅ Created {panelName}");
        return panel;
    }
    
    void CreateButton(GameObject parent, string buttonText, Vector2 anchorPosition, string targetObjectName, string methodName)
    {
        GameObject buttonObj = new GameObject($"Button_{buttonText}");
        buttonObj.transform.SetParent(parent.transform, false);
        
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchorMin = anchorPosition;
        buttonRect.anchorMax = anchorPosition;
        buttonRect.sizeDelta = new Vector2(200, 60);
        
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.3f, 0.3f, 0.8f, 1f);
        
        Button button = buttonObj.AddComponent<Button>();
        
        // Add text to button
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI buttonTextComp = textObj.AddComponent<TextMeshProUGUI>();
        buttonTextComp.text = buttonText;
        buttonTextComp.fontSize = 24;
        buttonTextComp.color = Color.white;
        buttonTextComp.alignment = TextAlignmentOptions.Center;
        
        // Setup button click - this would need to be done manually in Inspector
        Debug.Log($"⚠️  Connect {buttonText} button to {targetObjectName}.{methodName}() manually in Inspector");
    }
    
    void CreateHUDElements()
    {
        // Find or create Gameplay Panel
        Transform gameplayPanel = uiCanvasObj.transform.Find("GameplayPanel");
        if (gameplayPanel == null) return;
        
        // Create Health Bar
        CreateHealthBar(gameplayPanel.gameObject);
        
        // Create Game Time Text
        CreateGameTimeText(gameplayPanel.gameObject);
        
        // Create Status Text
        CreateStatusText(gameplayPanel.gameObject);
    }
    
    void CreateHealthBar(GameObject parent)
    {
        GameObject healthBarObj = new GameObject("HealthBar");
        healthBarObj.transform.SetParent(parent.transform, false);
        
        RectTransform healthRect = healthBarObj.AddComponent<RectTransform>();
        healthRect.anchorMin = new Vector2(0.1f, 0.9f);
        healthRect.anchorMax = new Vector2(0.5f, 0.95f);
        healthRect.offsetMin = Vector2.zero;
        healthRect.offsetMax = Vector2.zero;
        
        Slider slider = healthBarObj.AddComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = 100;
        slider.value = 100;
        
        // Create background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(healthBarObj.transform, false);
        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        // Create fill area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(healthBarObj.transform, false);
        RectTransform fillRect = fillArea.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        RectTransform fillImageRect = fill.AddComponent<RectTransform>();
        fillImageRect.anchorMin = Vector2.zero;
        fillImageRect.anchorMax = Vector2.one;
        fillImageRect.offsetMin = Vector2.zero;
        fillImageRect.offsetMax = Vector2.zero;
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = Color.green;
        
        slider.fillRect = fillImageRect;
        
        Debug.Log("✅ Created Health Bar");
    }
    
    void CreateGameTimeText(GameObject parent)
    {
        GameObject timeTextObj = new GameObject("GameTimeText");
        timeTextObj.transform.SetParent(parent.transform, false);
        
        RectTransform timeRect = timeTextObj.AddComponent<RectTransform>();
        timeRect.anchorMin = new Vector2(0.8f, 0.9f);
        timeRect.anchorMax = new Vector2(0.95f, 0.95f);
        timeRect.offsetMin = Vector2.zero;
        timeRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI timeText = timeTextObj.AddComponent<TextMeshProUGUI>();
        timeText.text = "Time: 00:00";
        timeText.fontSize = 24;
        timeText.color = Color.white;
        timeText.alignment = TextAlignmentOptions.Center;
        
        Debug.Log("✅ Created Game Time Text");
    }
    
    void CreateStatusText(GameObject parent)
    {
        GameObject statusTextObj = new GameObject("StatusText");
        statusTextObj.transform.SetParent(parent.transform, false);
        
        RectTransform statusRect = statusTextObj.AddComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0.3f, 0.8f);
        statusRect.anchorMax = new Vector2(0.7f, 0.85f);
        statusRect.offsetMin = Vector2.zero;
        statusRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI statusText = statusTextObj.AddComponent<TextMeshProUGUI>();
        statusText.text = "Ready for Battle!";
        statusText.fontSize = 28;
        statusText.color = Color.yellow;
        statusText.alignment = TextAlignmentOptions.Center;
        
        Debug.Log("✅ Created Status Text");
    }
    
    void SetupCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            mainCamera = camObj.AddComponent<Camera>();
            camObj.AddComponent<AudioListener>();
            Debug.Log("✅ Created Main Camera");
        }
        
        // Add CameraController if not present
        CameraController camController = mainCamera.GetComponent<CameraController>();
        if (camController == null)
        {
            mainCamera.gameObject.AddComponent<CameraController>();
            Debug.Log("✅ Added CameraController to Main Camera");
        }
        
        // Setup camera for 2D
        mainCamera.orthographic = true;
        mainCamera.orthographicSize = 8f;
        mainCamera.transform.position = new Vector3(0, 0, -10);
        
        Debug.Log("✅ Configured Main Camera for WorldOfBalance");
    }
    
    void SetupTanks()
    {
        // Find existing tanks
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.ToLower().Contains("player"))
            {
                SetupPlayerTank(obj);
            }
            else if (obj.name.ToLower().Contains("enemy"))
            {
                SetupEnemyTank(obj);
            }
        }
    }
    
    void SetupPlayerTank(GameObject playerTank)
    {
        // Ensure Player tag
        playerTank.tag = "Player";
        
        // Ensure required components
        if (playerTank.GetComponent<PlayerController>() == null)
            playerTank.AddComponent<PlayerController>();
            
        if (playerTank.GetComponent<TankController>() == null)
            playerTank.AddComponent<TankController>();
            
        if (playerTank.GetComponent<Rigidbody2D>() == null)
            playerTank.AddComponent<Rigidbody2D>();
            
        if (playerTank.GetComponent<CircleCollider2D>() == null)
            playerTank.AddComponent<CircleCollider2D>();
        
        // Setup visual (simple colored quad)
        if (playerTank.GetComponent<SpriteRenderer>() == null)
        {
            SpriteRenderer sr = playerTank.AddComponent<SpriteRenderer>();
            sr.color = Color.blue;
        }
        
        Debug.Log($"✅ Setup Player Tank: {playerTank.name}");
    }
    
    void SetupEnemyTank(GameObject enemyTank)
    {
        // Ensure Enemy tag
        enemyTank.tag = "Enemy";
        
        // Ensure required components
        if (enemyTank.GetComponent<EnemyAI>() == null)
            enemyTank.AddComponent<EnemyAI>();
            
        if (enemyTank.GetComponent<TankController>() == null)
            enemyTank.AddComponent<TankController>();
            
        if (enemyTank.GetComponent<Rigidbody2D>() == null)
            enemyTank.AddComponent<Rigidbody2D>();
            
        if (enemyTank.GetComponent<CircleCollider2D>() == null)
            enemyTank.AddComponent<CircleCollider2D>();
        
        // Setup visual
        if (enemyTank.GetComponent<SpriteRenderer>() == null)
        {
            SpriteRenderer sr = enemyTank.AddComponent<SpriteRenderer>();
            sr.color = Color.red;
        }
        
        Debug.Log($"✅ Setup Enemy Tank: {enemyTank.name}");
    }
    
    void SetupArena()
    {
        // Create simple arena walls if they don't exist
        if (GameObject.Find("Arena") == null)
        {
            GameObject arena = new GameObject("Arena");
            
            CreateWall("TopWall", new Vector3(0, 10, 0), new Vector3(25, 1, 1));
            CreateWall("BottomWall", new Vector3(0, -10, 0), new Vector3(25, 1, 1));
            CreateWall("LeftWall", new Vector3(-12, 0, 0), new Vector3(1, 20, 1));
            CreateWall("RightWall", new Vector3(12, 0, 0), new Vector3(1, 20, 1));
            
            Debug.Log("✅ Created Arena walls");
        }
        else
        {
            Debug.Log("✅ Arena already exists");
        }
    }
    
    void CreateWall(string wallName, Vector3 position, Vector3 scale)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = wallName;
        wall.tag = "Wall";
        wall.transform.position = position;
        wall.transform.localScale = scale;
        
        // Make it 2D
        Destroy(wall.GetComponent<Collider>());
        wall.AddComponent<BoxCollider2D>();
        
        // Gray color
        wall.GetComponent<Renderer>().material.color = Color.gray;
    }
}