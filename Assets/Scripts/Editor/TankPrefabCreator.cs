using UnityEngine;
using UnityEditor;
using Mirror;

public class TankPrefabCreator : EditorWindow
{
    [MenuItem("Tools/Create Tank Prefab")]
    public static void CreateTankPrefab()
    {
        // Create main tank object
        GameObject tank = new GameObject("Tank");
        
        // Add NetworkIdentity
        tank.AddComponent<NetworkIdentity>();
        
        // Add SpriteRenderer
        SpriteRenderer spriteRenderer = tank.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/tank_sprite.png");
        if (spriteRenderer.sprite == null)
        {
            // Create a default sprite if none exists
            spriteRenderer.sprite = CreateDefaultSprite();
        }
        spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 1f);
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
        turretSprite.sprite = spriteRenderer.sprite;
        turretSprite.color = new Color(0, 0, 0.5f, 1f);
        turretSprite.sortingOrder = 1;
        
        // Set tag
        tank.tag = "Player";
        
        // Create prefab
        string prefabPath = "Assets/Prefabs/Tank.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(tank, prefabPath);
        
        // Clean up scene object
        DestroyImmediate(tank);
        
        Debug.Log($"Tank prefab created at {prefabPath}");
        
        // Select the prefab
        Selection.activeObject = prefab;
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