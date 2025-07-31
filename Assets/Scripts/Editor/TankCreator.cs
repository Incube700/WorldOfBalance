using UnityEngine;
using UnityEditor;

public class TankCreator : EditorWindow
{
    [MenuItem("Tools/Create Tank in Scene")]
    public static void CreateTankInScene()
    {
        // Create main tank object
        GameObject tank = new GameObject("Tank");
        
        // Add SpriteRenderer
        SpriteRenderer spriteRenderer = tank.AddComponent<SpriteRenderer>();
        
        // Try to find a square sprite, or create a default one
        Sprite squareSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Square.png");
        if (squareSprite == null)
        {
            // Try to find any sprite in the project
            string[] spriteGuids = AssetDatabase.FindAssets("t:Sprite");
            if (spriteGuids.Length > 0)
            {
                string spritePath = AssetDatabase.GUIDToAssetPath(spriteGuids[0]);
                squareSprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            }
        }
        
        if (squareSprite == null)
        {
            // Create a default white square sprite
            squareSprite = CreateDefaultSprite();
        }
        
        spriteRenderer.sprite = squareSprite;
        spriteRenderer.color = new Color(0, 0.5f, 1f, 1f); // Blue color
        spriteRenderer.sortingOrder = 0;
        
        // Add Rigidbody2D
        Rigidbody2D rb = tank.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearDamping = 0.5f;
        rb.angularDamping = 0.05f;
        rb.gravityScale = 0f;
        rb.collisionDetection = CollisionDetectionMode2D.Continuous;
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        // Add BoxCollider2D
        BoxCollider2D boxCollider = tank.AddComponent<BoxCollider2D>();
        boxCollider.size = new Vector2(1f, 1f);
        boxCollider.isTrigger = false;
        
        // Add TankController
        tank.AddComponent<TankController>();
        
        // Add HealthSystem
        tank.AddComponent<HealthSystem>();
        
        // Add ArmorSystem
        tank.AddComponent<ArmorSystem>();
        
        // Add ProjectileSpawner
        tank.AddComponent<ProjectileSpawner>();
        
        // Add PlayerController for local control
        tank.AddComponent<PlayerController>();
        
        // Create turret child
        GameObject turret = new GameObject("Turret");
        turret.transform.SetParent(tank.transform);
        turret.transform.localPosition = new Vector3(0, 0.5f, 0);
        turret.transform.localScale = new Vector3(0.3f, 1.2f, 1f);
        
        // Add turret SpriteRenderer
        SpriteRenderer turretSprite = turret.AddComponent<SpriteRenderer>();
        turretSprite.sprite = squareSprite;
        turretSprite.color = new Color(0, 0, 0.5f, 1f);
        turretSprite.sortingOrder = 1;
        
        // Set tag
        tank.tag = "Player";
        
        // Position tank in scene
        tank.transform.position = Vector3.zero;
        
        // Select the tank in hierarchy
        Selection.activeGameObject = tank;
        
        Debug.Log("✅ Tank created in scene with all components!");
        Debug.Log("📋 Components added:");
        Debug.Log("  - SpriteRenderer (blue square)");
        Debug.Log("  - Rigidbody2D (Dynamic, Drag 0.5, Gravity 0)");
        Debug.Log("  - BoxCollider2D (1x1)");
        Debug.Log("  - TankController (MonoBehaviour)");
        Debug.Log("  - HealthSystem");
        Debug.Log("  - ArmorSystem");
        Debug.Log("  - ProjectileSpawner");
        Debug.Log("  - PlayerController (for local control)");
    }
    
    [MenuItem("Tools/Create Tank Prefab from Scene")]
    public static void CreateTankPrefabFromScene()
    {
        // Find tank in scene
        GameObject tankInScene = GameObject.Find("Tank");
        
        if (tankInScene == null)
        {
            Debug.LogError("❌ No Tank found in scene! Create one first using 'Create Tank in Scene'");
            return;
        }
        
        // Create prefab
        string prefabPath = "Assets/Prefabs/Tank.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(tankInScene, prefabPath);
        
        Debug.Log($"✅ Tank prefab created at {prefabPath}");
        
        // Select the prefab
        Selection.activeObject = prefab;
    }
    
    [MenuItem("Tools/Delete Tank from Scene")]
    public static void DeleteTankFromScene()
    {
        // Find tank in scene
        GameObject tankInScene = GameObject.Find("Tank");
        
        if (tankInScene == null)
        {
            Debug.LogWarning("⚠️ No Tank found in scene to delete");
            return;
        }
        
        // Delete tank from scene
        DestroyImmediate(tankInScene);
        
        Debug.Log("✅ Tank deleted from scene (Mirror will spawn it automatically)");
    }
    
    [MenuItem("Tools/Setup Game Manager")]
    public static void SetupGameManager()
    {
        // Find or create GameManager
        GameObject gameManager = GameObject.Find("GameManager");
        
        if (gameManager == null)
        {
            gameManager = new GameObject("GameManager");
            Debug.Log("✅ GameManager created");
        }
        
        // Select GameManager
        Selection.activeGameObject = gameManager;
    }
    
    [MenuItem("Tools/Complete Tank Setup")]
    public static void CompleteTankSetup()
    {
        Debug.Log("🚀 Starting complete Tank setup...");
        
        // Step 1: Create Tank in scene
        CreateTankInScene();
        
        // Step 2: Create prefab
        CreateTankPrefabFromScene();
        
        // Step 3: Delete from scene
        DeleteTankFromScene();
        
        // Step 4: Setup GameManager
        SetupGameManager();
        
        Debug.Log("🎉 Complete Tank setup finished!");
        Debug.Log("🧪 Ready to test:");
        Debug.Log("  1. Press Play");
        Debug.Log("  2. Tank should spawn and move");
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