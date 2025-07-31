using UnityEngine;
using UnityEditor;
using Mirror;

public class TankCreator : EditorWindow
{
    [MenuItem("Tools/Create Tank in Scene")]
    public static void CreateTankInScene()
    {
        // Create main tank object
        GameObject tank = new GameObject("Tank");
        
        // Add NetworkIdentity
        tank.AddComponent<NetworkIdentity>();
        
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
        rb.drag = 0.5f;
        rb.angularDrag = 0.05f;
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
        
        Debug.Log("Tank created in scene with all components!");
    }
    
    [MenuItem("Tools/Create Tank Prefab from Scene")]
    public static void CreateTankPrefabFromScene()
    {
        // Find tank in scene
        GameObject tankInScene = GameObject.Find("Tank");
        
        if (tankInScene == null)
        {
            Debug.LogError("No Tank found in scene! Create one first using 'Create Tank in Scene'");
            return;
        }
        
        // Create prefab
        string prefabPath = "Assets/Prefabs/Tank.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(tankInScene, prefabPath);
        
        Debug.Log($"Tank prefab created at {prefabPath}");
        
        // Select the prefab
        Selection.activeObject = prefab;
    }
    
    [MenuItem("Tools/Setup NetworkManager")]
    public static void SetupNetworkManager()
    {
        // Find or create NetworkManager
        GameNetworkManager networkManager = FindObjectOfType<GameNetworkManager>();
        
        if (networkManager == null)
        {
            GameObject networkManagerObj = new GameObject("NetworkManager");
            networkManager = networkManagerObj.AddComponent<GameNetworkManager>();
            
            // Add NetworkManagerHUD
            networkManagerObj.AddComponent<NetworkManagerHUD>();
        }
        
        // Set player prefab
        GameObject tankPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Tank.prefab");
        if (tankPrefab != null)
        {
            networkManager.playerPrefab = tankPrefab;
            Debug.Log("NetworkManager configured with Tank.prefab");
        }
        else
        {
            Debug.LogWarning("Tank.prefab not found! Create it first.");
        }
        
        // Select NetworkManager
        Selection.activeGameObject = networkManager.gameObject;
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