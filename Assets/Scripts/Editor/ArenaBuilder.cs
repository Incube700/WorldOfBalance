using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Linq;

public class ArenaBuilder : MonoBehaviour
{
    [MenuItem("Tools/Create Tank Arena")]
    public static void CreateTankArena()
    {
        Debug.Log("=== СОЗДАЕМ АРЕНУ ДЛЯ ТАНКОВ ===");
        
        // Clear existing arena objects
        ClearExistingArena();
        
        // Create arena components
        CreateArenaWalls();
        CreatePlayerTank();
        CreateEnemyTank();
        SetupCamera();
        
        // Create health UI
        CreateHealthUI();
        
        Debug.Log("=== АРЕНА ГОТОВА! ===");
    }
    
    static void ClearExistingArena()
    {
        // Remove existing arena objects
        GameObject[] wallObjects = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wall in wallObjects)
        {
            DestroyImmediate(wall);
        }
        
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in playerObjects)
        {
            DestroyImmediate(player);
        }
        
        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
        foreach (EnemyAI enemy in enemies)
        {
            DestroyImmediate(enemy.gameObject);
        }
    }
    
    static void CreateArenaWalls()
    {
        Debug.Log("Создаем стены арены...");
        
        // Arena dimensions
        float arenaWidth = 20f;
        float arenaHeight = 12f;
        float wallThickness = 1f;
        
        // Create Physics Material for bouncy walls
        PhysicsMaterial2D bouncyMaterial = new PhysicsMaterial2D("BouncyWall");
        bouncyMaterial.friction = 0f;
        bouncyMaterial.bounciness = 1f;
        
        // Save the material as an asset
        if (!AssetDatabase.IsValidFolder("Assets/Physics"))
        {
            AssetDatabase.CreateFolder("Assets", "Physics");
        }
        AssetDatabase.CreateAsset(bouncyMaterial, "Assets/Physics/BouncyWall.physicsMaterial2D");
        
        // Top wall
        CreateWall("TopWall", new Vector3(0, arenaHeight/2, 0), new Vector3(arenaWidth, wallThickness, 1), bouncyMaterial);
        
        // Bottom wall
        CreateWall("BottomWall", new Vector3(0, -arenaHeight/2, 0), new Vector3(arenaWidth, wallThickness, 1), bouncyMaterial);
        
        // Left wall
        CreateWall("LeftWall", new Vector3(-arenaWidth/2, 0, 0), new Vector3(wallThickness, arenaHeight, 1), bouncyMaterial);
        
        // Right wall
        CreateWall("RightWall", new Vector3(arenaWidth/2, 0, 0), new Vector3(wallThickness, arenaHeight, 1), bouncyMaterial);
        
        Debug.Log("Стены арены созданы");
    }
    
    static void CreateWall(string name, Vector3 position, Vector3 scale, PhysicsMaterial2D material)
    {
        GameObject wall = new GameObject(name);
        wall.tag = "Wall";
        wall.transform.position = position;
        wall.transform.localScale = scale;
        
        // Visual
        SpriteRenderer spriteRenderer = wall.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateSquareSprite();
        spriteRenderer.color = Color.white; // White walls as specified
        spriteRenderer.sortingOrder = -1;
        
        // Physics
        BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
        collider.sharedMaterial = material;
        
        Rigidbody2D rigidbody = wall.AddComponent<Rigidbody2D>();
        rigidbody.bodyType = RigidbodyType2D.Static;
    }
    
    static void CreatePlayerTank()
    {
        Debug.Log("Создаем танк игрока...");
        
        GameObject player = new GameObject("Player");
        player.tag = "Player";
        player.transform.position = new Vector3(-7, 0, 0); // Left side as specified
        
        // Visual - Blue tank (bigger size for better visibility)
        SpriteRenderer spriteRenderer = player.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateSquareSprite();
        spriteRenderer.color = Color.blue; // Blue tank as specified
        spriteRenderer.sortingOrder = 1;
        player.transform.localScale = new Vector3(2f, 2f, 1f); // Make tank bigger and more visible
        
        // Physics - hitbox matches visual size
        BoxCollider2D collider = player.AddComponent<BoxCollider2D>();
        collider.isTrigger = false;
        collider.size = new Vector2(1.1f, 1.1f); // Slightly bigger than visual for easier hits
        
        Rigidbody2D rigidbody = player.AddComponent<Rigidbody2D>();
        rigidbody.bodyType = RigidbodyType2D.Dynamic; // Dynamic for floaty physics
        rigidbody.gravityScale = 0f;
        rigidbody.linearDamping = 2f; // Add drag for inertia feel
        rigidbody.angularDamping = 5f;
        rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        // Components
        player.AddComponent<PlayerController>();
        player.AddComponent<HealthSystem>();
        player.AddComponent<Weapon>();
        
        // Create turret
        GameObject turret = new GameObject("PlayerTurret");
        turret.transform.SetParent(player.transform);
        turret.transform.localPosition = Vector3.zero;
        
        SpriteRenderer turretSprite = turret.AddComponent<SpriteRenderer>();
        turretSprite.sprite = CreateSquareSprite();
        turretSprite.color = new Color(0, 0, 0.8f, 1f); // Dark blue turret
        turretSprite.sortingOrder = 2;
        turretSprite.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
        
        // Fire point - positioned at the tip of the tank barrel
        GameObject firePoint = new GameObject("FirePoint");
        firePoint.transform.SetParent(player.transform);
        firePoint.transform.localPosition = new Vector3(0.6f, 0, 0); // At barrel tip (adjusted for bigger tank)
        
        Debug.Log("Танк игрока создан");
    }
    
    static void CreateEnemyTank()
    {
        Debug.Log("Создаем танк врага...");
        
        GameObject enemy = new GameObject("Enemy");
        enemy.transform.position = new Vector3(7, 0, 0); // Right side as specified
        
        // Visual - Red tank (bigger size for better visibility)
        SpriteRenderer spriteRenderer = enemy.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateSquareSprite();
        spriteRenderer.color = Color.red; // Red tank as specified
        spriteRenderer.sortingOrder = 1;
        enemy.transform.localScale = new Vector3(2f, 2f, 1f); // Make tank bigger and more visible
        
        // Physics - slightly bigger hitbox for easier targeting
        BoxCollider2D collider = enemy.AddComponent<BoxCollider2D>();
        collider.isTrigger = false;
        collider.size = new Vector2(1.2f, 1.2f); // Even bigger hitbox for enemy (easier to hit)
        
        Rigidbody2D rigidbody = enemy.AddComponent<Rigidbody2D>();
        rigidbody.bodyType = RigidbodyType2D.Dynamic; // Dynamic for floaty physics
        rigidbody.gravityScale = 0f;
        rigidbody.linearDamping = 2f; // Add drag for inertia feel
        rigidbody.angularDamping = 5f;
        rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        // Components
        enemy.AddComponent<EnemyAI>();
        enemy.AddComponent<HealthSystem>();
        
        // Create turret
        GameObject turret = new GameObject("EnemyTurret");
        turret.transform.SetParent(enemy.transform);
        turret.transform.localPosition = Vector3.zero;
        
        SpriteRenderer turretSprite = turret.AddComponent<SpriteRenderer>();
        turretSprite.sprite = CreateSquareSprite();
        turretSprite.color = new Color(0.8f, 0, 0, 1f); // Dark red turret
        turretSprite.sortingOrder = 2;
        turretSprite.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
        
        // Fire point - positioned at the tip of the enemy tank barrel
        GameObject firePoint = new GameObject("FirePoint");
        firePoint.transform.SetParent(enemy.transform);
        firePoint.transform.localPosition = new Vector3(0.6f, 0, 0); // At barrel tip (adjusted for bigger tank)
        
        Debug.Log("Танк врага создан");
    }
    
    static void SetupCamera()
    {
        Debug.Log("Настраиваем камеру...");
        
        Camera camera = Camera.main;
        if (camera == null)
        {
            GameObject cameraObj = new GameObject("Main Camera");
            camera = cameraObj.AddComponent<Camera>();
            cameraObj.AddComponent<AudioListener>();
        }
        
        // Fixed top-down camera as specified
        camera.orthographic = true;
        camera.orthographicSize = 8f;
        camera.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 1f); // Dark background
        camera.transform.position = new Vector3(0, 0, -10);
        camera.transform.rotation = Quaternion.identity;
        
        Debug.Log("Камера настроена");
    }
    
    static void CreateHealthUI()
    {
        Debug.Log("Создаем UI здоровья...");
        
        // Create Canvas
        GameObject canvasObj = new GameObject("HealthCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Add HealthUI component to manage health bars
        HealthUI healthUI = canvasObj.AddComponent<HealthUI>();
        
        // Player Health (Top Left)
        CreateHealthBar("PlayerHealthBar", canvasObj.transform, new Vector2(-800, 450), Color.blue, "Player HP");
        
        // Enemy Health (Top Right)  
        CreateHealthBar("EnemyHealthBar", canvasObj.transform, new Vector2(800, 450), Color.red, "Enemy HP");
        
        Debug.Log("UI здоровья создан");
    }
    
    static void CreateHealthBar(string name, Transform parent, Vector2 position, Color color, string labelText)
    {
        // Health Bar Container
        GameObject healthBarObj = new GameObject(name);
        healthBarObj.transform.SetParent(parent);
        
        RectTransform rectTransform = healthBarObj.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = new Vector2(200, 60);
        
        // Background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(healthBarObj.transform);
        
        RectTransform bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        UnityEngine.UI.Image bgImage = bgObj.AddComponent<UnityEngine.UI.Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        // Health Fill
        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(healthBarObj.transform);
        
        RectTransform fillRect = fillObj.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = new Vector2(5, 5);
        fillRect.offsetMax = new Vector2(-5, -25);
        
        UnityEngine.UI.Image fillImage = fillObj.AddComponent<UnityEngine.UI.Image>();
        fillImage.color = color;
        fillImage.type = UnityEngine.UI.Image.Type.Filled;
        fillImage.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
        
        // Label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(healthBarObj.transform);
        
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0);
        labelRect.anchorMax = new Vector2(1, 0.3f);
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        
        UnityEngine.UI.Text labelComponent = labelObj.AddComponent<UnityEngine.UI.Text>();
        labelComponent.text = labelText;
        labelComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        labelComponent.fontSize = 14;
        labelComponent.color = Color.white;
        labelComponent.alignment = TextAnchor.MiddleCenter;
    }
    
    static Sprite CreateSquareSprite()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
    }
}