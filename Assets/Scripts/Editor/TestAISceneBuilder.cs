using UnityEngine;
using UnityEditor;

public class TestAISceneBuilder : MonoBehaviour
{
    [MenuItem("Tools/Create TestAI Scene")]
    public static void CreateTestAIScene()
    {
        Debug.Log("=== СОЗДАЕМ СЦЕНУ TESTAISCENE ===");
        
        // Создаем новую сцену
        CreateNewScene();
        
        // Создаем объекты
        CreateCamera();
        CreateGround();
        CreatePlayer();
        CreateEnemy();
        CreateProjectile();
        CreateGameManager();
        
        // Сохраняем сцену
        SaveScene();
        
        Debug.Log("=== СЦЕНА TESTAISCENE ГОТОВА! ===");
        Debug.Log("Сцена сохранена как TESTAISCENE");
    }
    
    static void CreateNewScene()
    {
        Debug.Log("Создаем новую сцену...");
        
        // Создаем новую сцену
        var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(
            UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects, 
            UnityEditor.SceneManagement.NewSceneMode.Single
        );
        
        // Удаляем стандартные объекты кроме камеры
        var allObjects = FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            if (obj.name != "Main Camera" && obj.name != "Directional Light")
            {
                DestroyImmediate(obj);
            }
        }
    }
    
    static void CreateCamera()
    {
        Debug.Log("Настраиваем камеру...");
        
        var camera = Camera.main;
        if (camera == null)
        {
            var cameraObj = new GameObject("Main Camera");
            camera = cameraObj.AddComponent<Camera>();
            cameraObj.AddComponent<AudioListener>();
        }
        
        camera.orthographic = true;
        camera.orthographicSize = 10f;
        camera.backgroundColor = new Color(0.2f, 0.3f, 0.5f, 1f);
        camera.transform.position = new Vector3(0, 0, -10);
        camera.clearFlags = CameraClearFlags.SolidColor;
        
        Debug.Log("Камера настроена");
    }
    
    static void CreateGround()
    {
        Debug.Log("Создаем землю...");
        
        var ground = new GameObject("Ground");
        ground.transform.position = new Vector3(0, -5, 0);
        ground.transform.localScale = new Vector3(20, 1, 1);
        
        var spriteRenderer = ground.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateSquareSprite();
        spriteRenderer.color = new Color(0.3f, 0.6f, 0.3f, 1f);
        spriteRenderer.sortingOrder = -1;
        
        var collider = ground.AddComponent<BoxCollider2D>();
        collider.isTrigger = false;
        
        var rigidbody = ground.AddComponent<Rigidbody2D>();
        rigidbody.bodyType = RigidbodyType2D.Static;
        rigidbody.gravityScale = 0f;
        
        Debug.Log("Земля создана");
    }
    
    static void CreatePlayer()
    {
        Debug.Log("Создаем игрока...");
        
        var player = new GameObject("Player");
        player.tag = "Player";
        player.transform.position = new Vector3(-5, 0, 0);
        
        // Визуал
        var spriteRenderer = player.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateSquareSprite();
        spriteRenderer.color = new Color(0, 0.5f, 1f, 1f);
        spriteRenderer.sortingOrder = 1;
        
        // Физика
        var collider = player.AddComponent<BoxCollider2D>();
        collider.isTrigger = false;
        
        var rigidbody = player.AddComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0f;
        rigidbody.drag = 0.5f;
        rigidbody.angularDrag = 0.05f;
        rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        // Компоненты
        var tankController = player.AddComponent<TankController>();
        var healthSystem = player.AddComponent<HealthSystem>();
        var armorSystem = player.AddComponent<ArmorSystem>();
        var projectileSpawner = player.AddComponent<ProjectileSpawner>();
        
        Debug.Log("Игрок создан");
    }
    
    static void CreateEnemy()
    {
        Debug.Log("Создаем врага...");
        
        var enemy = new GameObject("Enemy");
        enemy.transform.position = new Vector3(5, 0, 0);
        
        // Визуал
        var spriteRenderer = enemy.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateSquareSprite();
        spriteRenderer.color = new Color(1f, 0.3f, 0.3f, 1f);
        spriteRenderer.sortingOrder = 1;
        
        // Физика
        var collider = enemy.AddComponent<BoxCollider2D>();
        collider.isTrigger = false;
        
        var rigidbody = enemy.AddComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0f;
        rigidbody.drag = 0.5f;
        rigidbody.angularDrag = 0.05f;
        rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        // Компоненты
        var enemyAI = enemy.AddComponent<EnemyAIController>();
        var healthSystem = enemy.AddComponent<HealthSystem>();
        var armorSystem = enemy.AddComponent<ArmorSystem>();
        
        // Создаем башню врага
        var turret = new GameObject("EnemyTurret");
        turret.transform.SetParent(enemy.transform);
        turret.transform.localPosition = Vector3.zero;
        
        var turretSprite = turret.AddComponent<SpriteRenderer>();
        turretSprite.sprite = CreateSquareSprite();
        turretSprite.color = new Color(0.8f, 0.2f, 0.2f, 1f);
        turretSprite.sortingOrder = 2;
        turretSprite.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
        
        var turretController = turret.AddComponent<EnemyTurretController>();
        
        Debug.Log("Враг создан");
    }
    
    static void CreateProjectile()
    {
        Debug.Log("Создаем снаряд...");
        
        var projectile = new GameObject("Projectile");
        projectile.transform.position = new Vector3(0, 10, 0); // Вне экрана
        
        // Визуал
        var spriteRenderer = projectile.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateCircleSprite();
        spriteRenderer.color = new Color(1f, 0.5f, 0f, 1f);
        spriteRenderer.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
        spriteRenderer.sortingOrder = 3;
        
        // Физика
        var collider = projectile.AddComponent<CircleCollider2D>();
        collider.isTrigger = false;
        collider.radius = 0.5f;
        
        var rigidbody = projectile.AddComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0f;
        rigidbody.drag = 0f;
        rigidbody.angularDrag = 0.05f;
        rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        // Компонент
        var projectileComponent = projectile.AddComponent<Projectile>();
        
        // Скрываем снаряд
        projectile.SetActive(false);
        
        Debug.Log("Снаряд создан");
    }
    
    static void CreateGameManager()
    {
        Debug.Log("Создаем GameManager...");
        
        var gameManager = new GameObject("LocalGameManager");
        var localGameManager = gameManager.AddComponent<LocalGameManager>();
        
        // Настройка GameManager
        if (localGameManager != null)
        {
            // Находим игрока и назначаем ссылки
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var tankController = player.GetComponent<TankController>();
                var healthSystem = player.GetComponent<HealthSystem>();
                
                // Используем reflection для установки приватных полей
                var playerField = typeof(LocalGameManager).GetField("player", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var healthField = typeof(LocalGameManager).GetField("playerHealth", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (playerField != null) playerField.SetValue(localGameManager, tankController);
                if (healthField != null) healthField.SetValue(localGameManager, healthSystem);
            }
        }
        
        Debug.Log("GameManager создан");
    }
    
    static void SaveScene()
    {
        Debug.Log("Сохраняем сцену...");
        
        // Создаем папку Scenes если её нет
        if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
        {
            AssetDatabase.CreateFolder("Assets", "Scenes");
        }
        
        // Сохраняем сцену
        var scenePath = "Assets/Scenes/TESTAISCENE.unity";
        var success = UnityEditor.SceneManagement.EditorSceneManager.SaveScene(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene(), 
            scenePath
        );
        
        if (success)
        {
            Debug.Log($"Сцена сохранена: {scenePath}");
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogError("Ошибка при сохранении сцены!");
        }
    }
    
    static Sprite CreateSquareSprite()
    {
        var texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
    }
    
    static Sprite CreateCircleSprite()
    {
        var texture = new Texture2D(32, 32);
        var center = new Vector2(16, 16);
        var radius = 16f;
        
        for (int x = 0; x < 32; x++)
        {
            for (int y = 0; y < 32; y++)
            {
                var distance = Vector2.Distance(new Vector2(x, y), center);
                if (distance <= radius)
                {
                    texture.SetPixel(x, y, Color.white);
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }
        
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
    }
} 