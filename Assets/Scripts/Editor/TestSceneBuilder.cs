using UnityEngine;
using UnityEditor;

public class TestSceneBuilder : EditorWindow
{
    [MenuItem("Tools/Setup TestScene for Local Battle")]
    public static void SetupTestScene()
    {
        Debug.Log("🚀 Setting up TestScene for local battle...");
        
        // Clear existing objects
        ClearScene();
        
        // Create Player
        CreatePlayer();
        
        // Create Enemy
        CreateEnemy();
        
        // Create Ground
        CreateGround();
        
        // Create Projectile Prefab
        CreateProjectilePrefab();
        
        // Setup Camera
        SetupCamera();
        
        Debug.Log("✅ TestScene setup complete!");
        Debug.Log("🎮 Ready for local battle:");
        Debug.Log("  - Player: WASD + Mouse to shoot");
        Debug.Log("  - Enemy: AI controlled");
        Debug.Log("  - Ground: Collision for projectiles");
    }
    
    private static void ClearScene()
    {
        // Clear all objects except camera
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name != "Main Camera" && obj.name != "Directional Light")
            {
                DestroyImmediate(obj);
            }
        }
    }
    
    private static void CreatePlayer()
    {
        // Create Player GameObject
        GameObject player = new GameObject("Player");
        player.tag = "Player";
        
        // Add components
        SpriteRenderer playerSprite = player.AddComponent<SpriteRenderer>();
        playerSprite.sprite = CreateDefaultSprite();
        playerSprite.color = new Color(0, 0.5f, 1f, 1f); // Blue
        
        Rigidbody2D playerRb = player.AddComponent<Rigidbody2D>();
        playerRb.bodyType = RigidbodyType2D.Dynamic;
        playerRb.drag = 0.5f;
        playerRb.gravityScale = 0f;
        
        BoxCollider2D playerCollider = player.AddComponent<BoxCollider2D>();
        playerCollider.size = new Vector2(1f, 1f);
        
        // Add scripts
        player.AddComponent<HealthSystem>();
        player.AddComponent<ArmorSystem>();
        player.AddComponent<ProjectileSpawner>();
        player.AddComponent<PlayerController>();
        
        // Position player
        player.transform.position = new Vector3(-5f, 0f, 0f);
        
        Debug.Log("✅ Player created");
    }
    
    private static void CreateEnemy()
    {
        // Create Enemy GameObject
        GameObject enemy = new GameObject("Enemy");
        enemy.tag = "Enemy";
        
        // Add components
        SpriteRenderer enemySprite = enemy.AddComponent<SpriteRenderer>();
        enemySprite.sprite = CreateDefaultSprite();
        enemySprite.color = new Color(1f, 0, 0, 1f); // Red
        
        Rigidbody2D enemyRb = enemy.AddComponent<Rigidbody2D>();
        enemyRb.bodyType = RigidbodyType2D.Dynamic;
        enemyRb.drag = 0.5f;
        enemyRb.gravityScale = 0f;
        
        BoxCollider2D enemyCollider = enemy.AddComponent<BoxCollider2D>();
        enemyCollider.size = new Vector2(1f, 1f);
        
        // Add scripts
        enemy.AddComponent<HealthSystem>();
        enemy.AddComponent<ArmorSystem>();
        enemy.AddComponent<ProjectileSpawner>();
        enemy.AddComponent<EnemyController>();
        
        // Position enemy
        enemy.transform.position = new Vector3(5f, 0f, 0f);
        
        Debug.Log("✅ Enemy created");
    }
    
    private static void CreateGround()
    {
        // Create Ground GameObject
        GameObject ground = new GameObject("Ground");
        
        // Add components
        SpriteRenderer groundSprite = ground.AddComponent<SpriteRenderer>();
        groundSprite.sprite = CreateDefaultSprite();
        groundSprite.color = new Color(0.3f, 0.3f, 0.3f, 1f); // Gray
        
        BoxCollider2D groundCollider = ground.AddComponent<BoxCollider2D>();
        groundCollider.size = new Vector2(20f, 1f);
        groundCollider.isTrigger = false;
        
        // Position ground
        ground.transform.position = new Vector3(0f, -8f, 0f);
        ground.transform.localScale = new Vector3(20f, 1f, 1f);
        
        Debug.Log("✅ Ground created");
    }
    
    private static void CreateProjectilePrefab()
    {
        // Create Projectile GameObject
        GameObject projectile = new GameObject("Projectile");
        
        // Add components
        SpriteRenderer projectileSprite = projectile.AddComponent<SpriteRenderer>();
        projectileSprite.sprite = CreateDefaultSprite();
        projectileSprite.color = new Color(1f, 1f, 0f, 1f); // Yellow
        
        Rigidbody2D projectileRb = projectile.AddComponent<Rigidbody2D>();
        projectileRb.bodyType = RigidbodyType2D.Dynamic;
        projectileRb.gravityScale = 0f;
        
        CircleCollider2D projectileCollider = projectile.AddComponent<CircleCollider2D>();
        projectileCollider.radius = 0.2f;
        
        // Add Projectile script
        projectile.AddComponent<Projectile>();
        
        // Save as prefab
        string prefabPath = "Assets/Prefabs/Projectile.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(projectile, prefabPath);
        DestroyImmediate(projectile);
        
        Debug.Log("✅ Projectile prefab created");
    }
    
    private static void SetupCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = 8f;
            mainCamera.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 1f);
        }
        
        Debug.Log("✅ Camera configured");
    }
    
    private static Sprite CreateDefaultSprite()
    {
        // Create a simple white square sprite
        Texture2D texture = new Texture2D(64, 64);
        Color[] pixels = new Color[64 * 64];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }
        texture.SetPixels(pixels);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
    }
} 