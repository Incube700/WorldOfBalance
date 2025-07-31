using UnityEngine;
using UnityEditor;
using Mirror;

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
        
        // Check NetworkManager
        ValidateNetworkManager();
        
        // Check scripts compilation
        ValidateScripts();
        
        Debug.Log("🎉 Tank setup validation completed!");
    }
    
    private static void ValidateTankPrefab(GameObject tankPrefab)
    {
        Debug.Log("📋 Validating Tank prefab components:");
        
        // Check NetworkIdentity
        NetworkIdentity networkIdentity = tankPrefab.GetComponent<NetworkIdentity>();
        if (networkIdentity == null)
        {
            Debug.LogError("❌ NetworkIdentity missing on Tank prefab");
        }
        else
        {
            Debug.Log("✅ NetworkIdentity found");
        }
        
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
                rb.drag == 0.5f &&
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
    
    private static void ValidateNetworkManager()
    {
        Debug.Log("🌐 Validating NetworkManager:");
        
        GameNetworkManager networkManager = FindObjectOfType<GameNetworkManager>();
        if (networkManager == null)
        {
            Debug.LogError("❌ GameNetworkManager not found in scene");
            return;
        }
        
        Debug.Log("✅ GameNetworkManager found");
        
        if (networkManager.playerPrefab == null)
        {
            Debug.LogError("❌ Player Prefab not set in NetworkManager");
        }
        else
        {
            Debug.Log("✅ Player Prefab configured");
        }
        
        NetworkManagerHUD hud = networkManager.GetComponent<NetworkManagerHUD>();
        if (hud == null)
        {
            Debug.LogWarning("⚠️ NetworkManagerHUD missing");
        }
        else
        {
            Debug.Log("✅ NetworkManagerHUD found");
        }
    }
    
    private static void ValidateScripts()
    {
        Debug.Log("📜 Validating scripts:");
        
        // Check if Mirror is available
        if (typeof(NetworkBehaviour) != null)
        {
            Debug.Log("✅ Mirror framework available");
        }
        else
        {
            Debug.LogError("❌ Mirror framework not available");
        }
        
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