using UnityEngine;
using UnityEditor;
using Mirror;

public class TestSceneSetup : EditorWindow
{
    [MenuItem("Tools/Setup Test Scene")]
    public static void SetupTestScene()
    {
        // Create NetworkManager
        GameObject networkManager = new GameObject("NetworkManager");
        GameNetworkManager gameNetworkManager = networkManager.AddComponent<GameNetworkManager>();
        
        // Add NetworkManagerHUD
        networkManager.AddComponent<NetworkManagerHUD>();
        
        // Set player prefab
        GameObject tankPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Tank.prefab");
        if (tankPrefab != null)
        {
            gameNetworkManager.playerPrefab = tankPrefab;
        }
        
        // Create spawn points
        CreateSpawnPoints();
        
        // Create walls
        CreateWalls();
        
        // Create camera
        CreateCamera();
        
        Debug.Log("Test scene setup completed!");
    }
    
    private static void CreateSpawnPoints()
    {
        GameObject spawnPointsParent = new GameObject("SpawnPoints");
        
        // Create spawn points
        Vector3[] spawnPositions = {
            new Vector3(-5, 0, 0),
            new Vector3(5, 0, 0),
            new Vector3(0, 5, 0),
            new Vector3(0, -5, 0)
        };
        
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            GameObject spawnPoint = new GameObject($"SpawnPoint_{i}");
            spawnPoint.transform.SetParent(spawnPointsParent.transform);
            spawnPoint.transform.position = spawnPositions[i];
        }
    }
    
    private static void CreateWalls()
    {
        GameObject wallsParent = new GameObject("Walls");
        
        // Create boundary walls
        CreateWall("Wall_Top", new Vector3(0, 10, 0), new Vector3(20, 1, 1));
        CreateWall("Wall_Bottom", new Vector3(0, -10, 0), new Vector3(20, 1, 1));
        CreateWall("Wall_Left", new Vector3(-10, 0, 0), new Vector3(1, 20, 1));
        CreateWall("Wall_Right", new Vector3(10, 0, 0), new Vector3(1, 20, 1));
        
        // Create some obstacles
        CreateWall("Obstacle_1", new Vector3(-3, 3, 0), new Vector3(2, 2, 1));
        CreateWall("Obstacle_2", new Vector3(3, -3, 0), new Vector3(2, 2, 1));
        CreateWall("Obstacle_3", new Vector3(0, 0, 0), new Vector3(1, 1, 1));
    }
    
    private static void CreateWall(string name, Vector3 position, Vector3 scale)
    {
        GameObject wall = new GameObject(name);
        wall.transform.position = position;
        wall.transform.localScale = scale;
        
        // Add SpriteRenderer
        SpriteRenderer spriteRenderer = wall.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateDefaultSprite();
        spriteRenderer.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        spriteRenderer.sortingOrder = -1;
        
        // Add BoxCollider2D
        BoxCollider2D boxCollider = wall.AddComponent<BoxCollider2D>();
        boxCollider.size = Vector2.one;
        boxCollider.isTrigger = false;
        
        // Add Rigidbody2D (static)
        Rigidbody2D rb = wall.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
        
        wall.tag = "Wall";
    }
    
    private static void CreateCamera()
    {
        GameObject camera = new GameObject("Main Camera");
        Camera cam = camera.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 8f;
        cam.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 1f);
        
        // Add camera follow script
        camera.AddComponent<CameraFollow>();
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

// Simple camera follow script
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, -10);
    public float smoothSpeed = 5f;
    
    void LateUpdate()
    {
        if (target == null)
        {
            // Find player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
            return;
        }
        
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
} 