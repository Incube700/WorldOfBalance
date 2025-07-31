using UnityEngine;
using UnityEditor;

public class TankValidator : EditorWindow
{
    [MenuItem("Tools/Validate Tank Setup")]
    public static void ValidateTankSetup()
    {
        Debug.Log("🔍 Validating Tank setup...");
        
        // Check if Tank prefab exists
        GameObject tankPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Tank.prefab");
        if (tankPrefab == null)
        {
            Debug.LogError("❌ Tank.prefab not found in Assets/Prefabs/");
            return;
        }
        
        Debug.Log("✅ Tank.prefab found");
        
        // Validate Tank prefab components
        ValidateTankPrefab(tankPrefab);
        
        // Check GameManager
        ValidateGameManager();
        
        // Check scripts compilation
        ValidateScripts();
        
        Debug.Log("🎉 Tank setup validation completed!");
    }
    
    private static void ValidateTankPrefab(GameObject tankPrefab)
    {
        Debug.Log("📋 Validating Tank prefab components:");
        

        
        // Check SpriteRenderer
        SpriteRenderer spriteRenderer = tankPrefab.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("❌ SpriteRenderer missing on Tank prefab");
        }
        else
        {
            Debug.Log("✅ SpriteRenderer found");
        }
        
        // Check Rigidbody2D
        Rigidbody2D rb = tankPrefab.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("❌ Rigidbody2D missing on Tank prefab");
        }
        else
        {
            if (rb.bodyType == RigidbodyType2D.Dynamic &&
                rb.linearDamping == 0.5f &&
                rb.gravityScale == 0f)
            {
                Debug.Log("✅ Rigidbody2D configured correctly");
            }
            else
            {
                Debug.LogWarning("⚠️ Rigidbody2D settings may need adjustment");
            }
        }
        
        // Check BoxCollider2D
        BoxCollider2D boxCollider = tankPrefab.GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            Debug.LogError("❌ BoxCollider2D missing on Tank prefab");
        }
        else
        {
            Debug.Log("✅ BoxCollider2D found");
        }
        
        // Check TankController
        TankController tankController = tankPrefab.GetComponent<TankController>();
        if (tankController == null)
        {
            Debug.LogError("❌ TankController missing on Tank prefab");
        }
        else
        {
            Debug.Log("✅ TankController found");
        }
        
        // Check HealthSystem
        HealthSystem healthSystem = tankPrefab.GetComponent<HealthSystem>();
        if (healthSystem == null)
        {
            Debug.LogError("❌ HealthSystem missing on Tank prefab");
        }
        else
        {
            Debug.Log("✅ HealthSystem found");
        }
        
        // Check ArmorSystem
        ArmorSystem armorSystem = tankPrefab.GetComponent<ArmorSystem>();
        if (armorSystem == null)
        {
            Debug.LogError("❌ ArmorSystem missing on Tank prefab");
        }
        else
        {
            Debug.Log("✅ ArmorSystem found");
        }
        
        // Check ProjectileSpawner
        ProjectileSpawner projectileSpawner = tankPrefab.GetComponent<ProjectileSpawner>();
        if (projectileSpawner == null)
        {
            Debug.LogError("❌ ProjectileSpawner missing on Tank prefab");
        }
        else
        {
            Debug.Log("✅ ProjectileSpawner found");
        }
    }
    
    private static void ValidateGameManager()
    {
        Debug.Log("🎮 Validating GameManager:");
        
        GameObject gameManager = GameObject.Find("GameManager");
        if (gameManager == null)
        {
            Debug.LogWarning("⚠️ GameManager not found in scene");
        }
        else
        {
            Debug.Log("✅ GameManager found");
        }
    }
    
    private static void ValidateScripts()
    {
        Debug.Log("📜 Validating scripts:");
        

        
        // Check if Input System is available
        if (typeof(UnityEngine.InputSystem.InputAction) != null)
        {
            Debug.Log("✅ Input System available");
        }
        else
        {
            Debug.LogWarning("⚠️ Input System not available");
        }
        
        // Check if all required scripts exist
        string[] requiredScripts = {
            "TankController",
            "HealthSystem", 
            "ArmorSystem",
            "ProjectileSpawner",
            "Projectile"
        };
        
        foreach (string scriptName in requiredScripts)
        {
            System.Type scriptType = System.Type.GetType(scriptName);
            if (scriptType == null)
            {
                Debug.LogWarning($"⚠️ {scriptName} script not found");
            }
            else
            {
                Debug.Log($"✅ {scriptName} script found");
            }
        }
    }
} 